using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Client;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Services.Builders.Requests;
using TestApi.Services.Services;
using VideoApi.Client;
using VideoApi.Contract.Responses;

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
        [OpenApiOperation("GetHearingByIdAsync")]
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
        [OpenApiOperation("GetHearingsByUsernameAsync")]
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
        [OpenApiOperation("CreateHearingAsync")]
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
        [OpenApiOperation("ConfirmHearingByIdAsync")]
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
        [OpenApiOperation("DeleteHearingByIdAsync")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteHearingByIdAsync(Guid hearingId)
        {
            _logger.LogDebug($"DeleteHearingByIdAsync {hearingId}");

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

        /// <summary>
        /// Updates suitability answers for the participant
        /// </summary>
        /// <param name="hearingId">Id of hearing</param>
        /// <param name="participantId">Id of participant</param>
        /// <param name="answers">A list of suitability answers to update</param>
        /// <returns>Http status</returns>
        [HttpPut("{hearingId}/participants/{participantId}/update-suitability-answers")]
        [OpenApiOperation("UpdateSuitabilityAnswersAsync")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateSuitabilityAnswersAsync(Guid hearingId, Guid participantId, [FromBody] List<SuitabilityAnswersRequest> answers)
        {
            _logger.LogDebug("UpdateSuitabilityAnswersAsync");

            try
            {
                await _bookingsApiClient.UpdateSuitabilityAnswersAsync(hearingId, participantId, answers);

                return NoContent();
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        /// Get suitability answers for the person
        /// </summary>
        /// <param name="username">Username of the person</param>
        /// <returns>List of suitability answer responses</returns>
        [HttpGet("get-suitability-answers/{username}")]
        [OpenApiOperation("GetSuitabilityAnswersAsync")]
        [ProducesResponseType(typeof(List<PersonSuitabilityAnswerResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetSuitabilityAnswersAsync(string username)
        {
            _logger.LogDebug("GetSuitabilityAnswersAsync");

            try
            {
                var response = await _bookingsApiClient.GetPersonSuitabilityAnswersAsync(username);

                return Ok(response);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        /// Get person by username
        /// </summary>
        /// <param name="username">Username of the person</param>
        /// <returns>List of suitability answer responses</returns>
        [HttpGet("person/{username}")]
        [OpenApiOperation("GetPersonByUsernameAsync")]
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPersonByUsernameAsync(string username)
        {
            _logger.LogDebug($"GetPersonByUsernameAsync {username}");

            try
            {
                var response = await _bookingsApiClient.GetPersonByUsernameAsync(username);

                return Ok(response);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        /// Get all hearings by default case type
        /// </summary>
        /// <returns>List of hearings by default type</returns>
        [HttpGet("all/hearings")]
        [OpenApiOperation("GetAllHearingsAsync")]
        [ProducesResponseType(typeof(List<BookingsHearingResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllHearingsAsync()
        {
            _logger.LogDebug($"GetAllHearingsAsync");

            try
            {
                const int LIMIT = HearingData.GET_HEARINGS_LIMIT;
                var types = new List<int> { HearingData.GENERIC_CASE_TYPE_ID_FROM_BOOKINGS_API };
                var response = await _bookingsApiClient.GetHearingsByTypesAsync(types, null, LIMIT);

                var hearings = new List<BookingsHearingResponse>();
                foreach (var day in response.Hearings)
                {
                    hearings.AddRange(day.Hearings);  
                }

                return Ok(hearings);
            }
            catch (BookingsApiException e)
            {
                return e.StatusCode == (int) HttpStatusCode.NotFound ? Ok(new List<BookingsHearingResponse>()) : StatusCode(e.StatusCode, e.Response);
            }
        }
    }
}