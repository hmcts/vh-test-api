using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Contracts
{
    public interface IBookingsApiService
    {
        Task UpdateBookingStatusPollingAsync(Guid hearingId, UpdateBookingStatusRequest request);
    }

    public class BookingsApiService : IBookingsApiService
    {
        // 4 retries ^2 will execute after 2 seconds, then 4, 8, then finally 16 (30 seconds in total)
        private const int RETRIES = 4;

        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly ILogger<BookingsApiService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public BookingsApiService(IBookingsApiClient bookingsApiClient, ILogger<BookingsApiService> logger)
        {
            _bookingsApiClient = bookingsApiClient;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<BookingsApiException>()
                .Or<Exception>()
                .WaitAndRetryAsync(RETRIES, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task UpdateBookingStatusPollingAsync(Guid hearingId, UpdateBookingStatusRequest request)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(() => _bookingsApiClient.UpdateBookingStatusAsync(hearingId, request));
            }
            catch (Exception e)
            {
                _logger.LogError($"Encountered error '{e.Message}' after {RETRIES ^ 2} seconds.");
                throw;
            }
        }
    }
}