using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using VideoApi.Client;
using VideoApi.Contract.Responses;

namespace TestApi.Services.Services
{
    public interface IVideoApiService
    {
        /// <summary>Polls for the conference and retrieves the conference details</summary>
        /// <returns>Conference details</returns>
        Task<ConferenceDetailsResponse> GetConferenceByHearingIdPolling(Guid hearingRefId);
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

        public async Task<ConferenceDetailsResponse> GetConferenceByHearingIdPolling(Guid hearingRefId)
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
                var result = await policy.ExecuteAsync(() => _videoApiClient.GetConferenceByHearingRefIdAsync(hearingRefId, false));
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Encountered error '{message}' after {timeout} seconds whilst polling for conference by hearing ref id {hearingRefId}.", e.Message, RETRIES ^ 2, hearingRefId);
                throw;
            }
        }
    }
}