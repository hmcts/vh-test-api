using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestApi.Contract.Requests;
using TestApi.Services.Builders.Requests;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Services.Contracts;

namespace TestApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class HearingsController : ControllerBase
    {
        private readonly ILogger<HearingsController> _logger;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly IBookingsApiService _bookingsApiService;
        private readonly IVideoApiClient _videoApiClient;
        private readonly IVideoApiService _videoApiService;

        public HearingsController(ILogger<HearingsController> logger, IBookingsApiClient bookingsApiClient,
            IBookingsApiService bookingsApiService, IVideoApiClient videoApiClient, IVideoApiService videoApiService)
        {
            _logger = logger;
            _bookingsApiClient = bookingsApiClient;
            _bookingsApiService = bookingsApiService;
            _videoApiClient = videoApiClient;
            _videoApiService = videoApiService;
        }

        /// <summary>
        ///     Get the details of a hearing by id
        /// </summary>
        /// <param name="hearingId">Id of the hearing</param>
        /// <returns>Full details of a hearing</returns>
        [HttpGet("{hearingId}", Name = nameof(GetHearingByIdAsync))]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetHearingByIdAsync(Guid hearingId)
        {
            _logger.LogDebug($"GetHearingByIdAsync {hearingId}");

            try
            {
                var response = await _bookingsApiClient.GetHearingDetailsByIdAsync(hearingId);
                return Ok(response);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///    Get list of all hearings for a given username
        /// </summary>
        /// <param name="username">Username of the participant</param>
        /// <returns>Full details of a hearing</returns>
        [HttpGet("username/{username}", Name = nameof(GetHearingsByUsernameAsync))]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetHearingsByUsernameAsync(string username)
        {
            _logger.LogDebug($"GetHearingsByUsernameAsync {username}");

            try
            {
                var response = await _bookingsApiClient.GetHearingsByUsernameAsync(username);
                return Ok(response);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///     Create a hearing
        /// </summary>
        /// <param name="request">Details of the new user</param>
        /// <returns>Full details of an allocated user</returns>
        [HttpPost]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateHearingAsync(CreateHearingRequest request)
        {
            _logger.LogDebug("CreateHearingAsync");

            var bookHearingRequest = new BookHearingRequestBuilder(request).Build();

            try
            {
                var response = await _bookingsApiClient.BookNewHearingAsync(bookHearingRequest);

                _logger.LogDebug($"New Hearing Created with id {response.Id}");

                return CreatedAtAction(nameof(CreateHearingAsync), new {hearingId = response.Id}, response);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///     Confirm hearing by id
        /// </summary>
        /// <param name="hearingId">Id of the hearing</param>
        /// <param name="request">Update the booking status details</param>
        /// <returns>Confirm a hearing</returns>
        [HttpPatch("{hearingId}", Name = nameof(ConfirmHearingByIdAsync))]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ConfirmHearingByIdAsync(Guid hearingId, UpdateBookingStatusRequest request) 
        {
            _logger.LogDebug($"ConfirmHearingByIdAsync {hearingId}");

            try
            {
                await _bookingsApiClient.GetHearingDetailsByIdAsync(hearingId);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            _logger.LogDebug($"Hearing with id {hearingId} retrieved");

            try
            {
                await _bookingsApiService.UpdateBookingStatusPollingAsync(hearingId, request);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            _logger.LogInformation($"Successfully confirmed hearing with id {hearingId}");

            try
            {
                var response = await _videoApiService.GetConferenceByHearingIdPollingAsync(hearingId);
                return Created(nameof(ConfirmHearingByIdAsync), response);
            }
            catch (VideoApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///     Delete hearing by id
        /// </summary>
        /// <param name="hearingId">Id of the hearing</param>
        /// <returns>Delete a hearing</returns>
        [HttpDelete("{hearingId}", Name = nameof(DeleteHearingByIdAsync))]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteHearingByIdAsync(Guid hearingId)
        {
            _logger.LogDebug($"DeleteHearingByIdAsync {hearingId}");

            var existingHearing = await GetHearingByIdAsync(hearingId);

            if (existingHearing == null) return NotFound();

            _logger.LogDebug($"Hearing with id {hearingId} retrieved");

            try
            {
                await _bookingsApiClient.RemoveHearingAsync(hearingId);

                _logger.LogInformation($"Successfully deleted hearing with id {hearingId}");
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            try
            {
                await _videoApiClient.DeleteAudioApplicationAsync(hearingId);

                _logger.LogInformation($"Successfully deleted audio application with hearing id {hearingId}");
            }
            catch (VideoApiException e)
            {
                if (e.StatusCode != (int) HttpStatusCode.NotFound) return StatusCode(e.StatusCode, e.Response);

                _logger.LogInformation($"No audio application found to delete with hearing id {hearingId}");
            }

            return NoContent();
        }
    }
}