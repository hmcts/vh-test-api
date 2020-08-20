using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class ConferencesController : ControllerBase
    {
        private readonly ILogger<ConferencesController> _logger;
        private readonly IVideoApiClient _videoApiClient;

        public ConferencesController(ILogger<ConferencesController> logger, IVideoApiClient videoApiClient)
        {
            _logger = logger;
            _videoApiClient = videoApiClient;
        }

        /// <summary>
        ///     Get the details of a conference by id
        /// </summary>
        /// <param name="conferenceId">Id of the conference</param>
        /// <returns>Full details of a conference</returns>
        [HttpGet("{conferenceId}", Name = nameof(GetConferenceByIdAsync))]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferenceByIdAsync(Guid conferenceId)
        {
            _logger.LogDebug($"GetConferenceByIdAsync {conferenceId}");

            try
            {
                var response = await _videoApiClient.GetConferenceDetailsByIdAsync(conferenceId);
                return Ok(response);
            }
            catch (VideoApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///     Get the details of a conference by hearing ref id
        /// </summary>
        /// <param name="hearingRefId">Hearing ref Id of the conference</param>
        /// <returns>Full details of a conference</returns>
        [HttpGet("hearings/{hearingRefId}", Name = nameof(GetConferenceByHearingRefIdAsync))]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferenceByHearingRefIdAsync(Guid hearingRefId)
        {
            _logger.LogDebug($"GetConferenceByHearingRefIdAsync {hearingRefId}");

            try
            {
                var response = await _videoApiClient.GetConferenceByHearingRefIdAsync(hearingRefId);
                return Ok(response);
            }
            catch (VideoApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        /// Request to book a conference
        /// </summary>
        /// <param name="request">Details of a conference</param>
        /// <returns>Details of the new conference</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "BookNewConference")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            _logger.LogDebug($"BookNewConferenceAsync");

            try
            {
                var response = await _videoApiClient.BookNewConferenceAsync(request);
                return Created(nameof(BookNewConferenceAsync), response);
            }
            catch (VideoApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///     Delete a conference by conference id
        /// </summary>
        /// <param name="hearingRefId">Hearing Ref Id of the conference</param>
        /// <param name="conferenceId">Conference Id of the conference</param>
        /// <returns></returns>
        [HttpDelete("{hearingRefId}/{conferenceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteConferenceAsync(Guid hearingRefId, Guid conferenceId)
        {
            _logger.LogDebug($"DeleteConferenceAsync {conferenceId}");

            try
            {
                await _videoApiClient.RemoveConferenceAsync(conferenceId);
            }
            catch (VideoApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            try
            {
                await _videoApiClient.DeleteAudioApplicationAsync(hearingRefId);

                _logger.LogInformation($"Successfully deleted audio application with hearing id {hearingRefId}");
            }
            catch (VideoApiException e)
            {
                if (e.StatusCode != (int)HttpStatusCode.NotFound) return StatusCode(e.StatusCode, e.Response);

                _logger.LogInformation($"No audio application found to delete with hearing id {hearingRefId}");
            }

            return NoContent();
        }

        /// <summary>
        ///     Get conferences for today Judge
        /// </summary>
        /// <param name="username">Username of the Judge</param>
        /// <returns>Full details of all conferences</returns>
        [HttpGet("today/judge", Name = nameof(GetConferencesForTodayJudgeAsync))]
        [ProducesResponseType(typeof(List<ConferenceDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesForTodayJudgeAsync(string username)
        {
            _logger.LogDebug($"GetConferencesForTodayJudgeAsync {username}");

            try
            {
                var response = await _videoApiClient.GetConferencesTodayForJudgeByUsernameAsync(username);
                return Ok(response);
            }
            catch (VideoApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///     Get conferences for today VHO
        /// </summary>
        /// <returns>Full details of all conferences</returns>
        [HttpGet("today/vho", Name = nameof(GetConferencesForTodayVhoAsync))]
        [ProducesResponseType(typeof(List<ConferenceDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesForTodayVhoAsync()
        {
            _logger.LogDebug($"GetConferencesForTodayVhoAsync");

            try
            {
                var response = await _videoApiClient.GetConferencesTodayForAdminAsync(new List<string>());
                return Ok(response);
            }
            catch (VideoApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }
    }
}
