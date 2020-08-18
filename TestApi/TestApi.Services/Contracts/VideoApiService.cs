using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.Services.Contracts
{
    public interface IVideoApiService
    {
        /// <summary>Polls for the conference and retrieves the conference details</summary>
        /// <returns>Conference details</returns>
        Task<ConferenceDetailsResponse> GetConferenceByHearingIdPollingAsync(Guid hearingRefId);
    }

    public class VideoApiService : IVideoApiService
    {
        private readonly ILogger<VideoApiService> _logger;
        private readonly IVideoApiClient _videoApiClient;

        public VideoApiService(IVideoApiClient videoApiClient, ILogger<VideoApiService> logger)
        {
            _videoApiClient = videoApiClient;
            _logger = logger;
        }

        public async Task<ConferenceDetailsResponse> GetConferenceByHearingIdPollingAsync(Guid hearingRefId)
        {
            // 4 retries ^2 will execute after 2 seconds, then 4, 8, then finally 16 (30 seconds in total)
            const int RETRIES = 4;

            var policy = Policy
                .Handle<VideoApiException>()
                .Or<Exception>()
                .WaitAndRetryAsync(RETRIES, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var result = await policy.ExecuteAsync(() => _videoApiClient.GetConferenceByHearingRefIdAsync(hearingRefId));
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Encountered error '{e.Message}' after {RETRIES^2} seconds.");
                throw;
            }
        }
    }
}