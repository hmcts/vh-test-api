using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Client;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using TestApi.Common.Data;
using TestApi.Contract.Requests;

namespace TestApi.Services.Services
{
    public interface IBookingsApiService
    {
        /// <summary>Updates booking status and checks conference has been created successfully in video api</summary>
        /// <param name="hearingId">Hearing id of the hearing</param>
        /// <param name="request">Update booking details</param>
        /// <returns></returns>
        Task UpdateBookingStatusPolling(Guid hearingId, UpdateBookingStatusRequest request);

        /// <summary>Delete all hearings by either case name or case number with partial text</summary>
        /// <param name="request">Partial case name or case number text for the hearing or conference</param>
        /// <returns>Number of hearings or conferences deleted</returns>
        Task<List<Guid>> DeleteHearingsByPartialCaseText(DeleteTestHearingDataRequest request);
    }

    public class BookingsApiService : IBookingsApiService
    {
        // 4 retries ^2 will execute after 2 seconds, then 4, 8, then finally 16 (30 seconds in total)
        private const int RETRIES = 4;
        private const string CURSOR = "0";
        private const int DEFAULT_LIMIT = 1000;
        private const string NAME_THAT_WONT_BE_FOUND = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
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

        public async Task UpdateBookingStatusPolling(Guid hearingId, UpdateBookingStatusRequest request)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(() => _bookingsApiClient.UpdateBookingStatusAsync(hearingId, request));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Encountered error '{message}' after {timeout} seconds.", e.Message, RETRIES ^ 2);
                throw;
            }
        }

        public async Task<List<Guid>> DeleteHearingsByPartialCaseText(DeleteTestHearingDataRequest request)
        {
            request.Limit ??= DEFAULT_LIMIT;

            try
            {
                var caseTypes = new List<int>{ HearingData.CACD_CASE_TYPE_ID_FROM_BOOKINGS_API, HearingData.GENERIC_CASE_TYPE_ID_FROM_BOOKINGS_API };
                var response = await _bookingsApiClient.GetHearingsByTypesAsync(caseTypes, CURSOR, DEFAULT_LIMIT);

                var hearings = new List<BookingsHearingResponse>();

                foreach (var bookedHearing in response.Hearings)
                {
                    hearings.AddRange(bookedHearing.Hearings);
                }

                return await DeleteHearings(hearings, request.PartialHearingCaseName);
            }
            catch (BookingsApiException e)
            {
                _logger.LogError(e, "Encountered error '{message}'", e.Message);
                throw;
            }
        }

        private async Task<List<Guid>> DeleteHearings(IEnumerable<BookingsHearingResponse> hearings, string hearingName)
        {
            if (hearingName.Equals(string.Empty))
            {
                hearingName = NAME_THAT_WONT_BE_FOUND;
            }

            var deletedHearingIds = new List<Guid>();
            foreach (var hearing in hearings)
            {
                if (!hearing.HearingName.ToLower().Contains(hearingName.ToLower())) continue;
                await _bookingsApiClient.RemoveHearingAsync(hearing.HearingId);
                deletedHearingIds.Add(hearing.HearingId);
                _logger.LogInformation("Deleted hearing with id {id}", hearing.HearingId);
            }

            return deletedHearingIds;
        }
    }
}