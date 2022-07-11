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
        [HttpGet("{hearingId}", Name = nameof(GetHearingById))]
        [OpenApiOperation("GetHearingById")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetHearingById(Guid hearingId)
        {
            _logger.LogDebug("GetHearingById {hearingId}", hearingId);

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
        [HttpGet("username/{username}", Name = nameof(GetHearingsByUsername))]
        [OpenApiOperation("GetHearingsByUsername")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetHearingsByUsername(string username)
        {
            _logger.LogDebug("GetHearingsByUsername {username}", username);

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
        [OpenApiOperation("CreateHearing")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateHearing(CreateHearingRequest request)
        {
            _logger.LogDebug("CreateHearing");

            var bookHearingRequest = new BookHearingRequestBuilder(request).Build();

            try
            {
                var response = await _bookingsApiClient.BookNewHearingAsync(bookHearingRequest);

                _logger.LogDebug("New Hearing Created with id {id}", response.Id);

                return CreatedAtAction(nameof(CreateHearing), new {hearingId = response.Id}, response);
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
        [HttpPatch("{hearingId}", Name = nameof(ConfirmHearingById))]
        [OpenApiOperation("ConfirmHearingById")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ConfirmHearingById(Guid hearingId, UpdateBookingStatusRequest request) 
        {
            _logger.LogDebug("ConfirmHearingById {hearingId}", hearingId);

            try
            {
                await _bookingsApiClient.GetHearingDetailsByIdAsync(hearingId);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            _logger.LogDebug("Hearing with id {hearingId} retrieved", hearingId);

            try
            {
                await _bookingsApiService.UpdateBookingStatusPolling(hearingId, request);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            _logger.LogInformation("Successfully confirmed hearing with id {hearingId}", hearingId);

            try
            {
                var response = await _videoApiService.GetConferenceByHearingIdPolling(hearingId);
                return Created(nameof(ConfirmHearingById), response);
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
        [HttpDelete("{hearingId}", Name = nameof(DeleteHearingById))]
        [OpenApiOperation("DeleteHearingById")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteHearingById(Guid hearingId)
        {
            _logger.LogDebug("DeleteHearingById {hearingId}", hearingId);

            try
            {
                await _bookingsApiClient.RemoveHearingAsync(hearingId);

                _logger.LogInformation("Successfully deleted hearing with id {hearingId}", hearingId);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            try
            {
                await _videoApiClient.DeleteAudioApplicationAsync(hearingId);

                _logger.LogInformation("Successfully deleted audio application with hearing id {hearingId}", hearingId);
            }
            catch (VideoApiException e)
            {
                if (e.StatusCode != (int) HttpStatusCode.NotFound) return StatusCode(e.StatusCode, e.Response);

                _logger.LogInformation("No audio application found to delete with hearing id {hearingId}", hearingId);
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
        [OpenApiOperation("UpdateSuitabilityAnswers")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateSuitabilityAnswers(Guid hearingId, Guid participantId, [FromBody] List<SuitabilityAnswersRequest> answers)
        {
            _logger.LogDebug("UpdateSuitabilityAnswers");

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
        [OpenApiOperation("GetSuitabilityAnswers")]
        [ProducesResponseType(typeof(List<PersonSuitabilityAnswerResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetSuitabilityAnswers(string username)
        {
            _logger.LogDebug("GetSuitabilityAnswers");

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
        [OpenApiOperation("GetPersonByUsername")]
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPersonByUsername(string username)
        {
            _logger.LogDebug("GetPersonByUsername {username}", username);

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
        [OpenApiOperation("GetAllHearings")]
        [ProducesResponseType(typeof(List<BookingsHearingResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllHearings()
        {
            _logger.LogDebug("GetAllHearings");

            try
            {
                var hearingRequest = new GetHearingRequest
                {
                    Types = new List<int> {HearingData.GENERIC_CASE_TYPE_ID_FROM_BOOKINGS_API},
                    Limit = HearingData.GET_HEARINGS_LIMIT
                };

                var response = await _bookingsApiClient.GetHearingsByTypesAsync(hearingRequest);

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