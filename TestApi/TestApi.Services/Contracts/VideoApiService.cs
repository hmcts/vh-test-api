using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Services.Exceptions;

namespace TestApi.Services.Contracts
{
    public interface IVideoApiService
    {
        /// <summary>Polls for the conference and retrieves the conference details</summary>
        /// <returns>Conference details</returns>
        Task<ConferenceDetailsResponse> GetConferenceByIdPollingAsync(Guid hearingRefId);
    }

    public class VideoApiService : IVideoApiService
    {
        private readonly IVideoApiClient _videoApiClient;
        private readonly ILogger<VideoApiService> _logger;

        public VideoApiService(IVideoApiClient videoApiClient, ILogger<VideoApiService> logger)
        {
            _videoApiClient = videoApiClient;
            _logger = logger;
        }

        public async Task<ConferenceDetailsResponse> GetConferenceByIdPollingAsync(Guid hearingRefId)
        {
            // 5 retries ^2 will execute after 2 seconds, then 4, 8, 16 then finally 32 (62 seconds total)
            const int RETRIES = 5;

            var policy = Policy
                .Handle<ConferenceNotCreatedException>()
                .WaitAndRetry(RETRIES, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, context) =>
                    {
                        _logger.LogWarning($"Encountered error '{exception.Message}' after {timeSpan.Seconds} seconds. Retrying...");
                    });

            var result = await policy.Execute(() => GetConference(hearingRefId));

            return result;
        }

        private async Task<ConferenceDetailsResponse> GetConference(Guid hearingRefId)
        {
            return await _videoApiClient.GetConferenceByHearingRefIdAsync(hearingRefId);
        }
    }
}
