using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.Services.Services;
using VideoApi.Client;

namespace TestApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("utilities")]
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        private readonly ILogger<UtilitiesController> _logger;
        private readonly IBookingsApiService _bookingsApiService;
        private readonly IVideoApiClient _videoApiClient;

        public UtilitiesController(ILogger<UtilitiesController> logger,
            IBookingsApiService bookingsApiService, IVideoApiClient videoApiClient)
        {
            _logger = logger;
            _bookingsApiService = bookingsApiService;
            _videoApiClient = videoApiClient;
        }

        /// <summary>
        ///    Delete hearings by partial case name or number
        /// </summary>
        /// <param name="request">Partial case name or number text for the hearing</param>
        /// <returns>Number of deleted hearings or conferences</returns>
        [HttpPost("removeTestData")]
        [OpenApiOperation("DeleteTestDataByPartialCaseText")]
        [ProducesResponseType(typeof(DeletedResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteTestDataByPartialCaseText(DeleteTestHearingDataRequest request)
        {
            _logger.LogDebug($"DeleteHearingsByPartialCaseText");

            List<Guid> hearingIds;

            try
            {
                hearingIds = await _bookingsApiService.DeleteHearingsByPartialCaseText(request);
            }
            catch (BookingsApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            try
            {
                foreach (var hearingId in hearingIds)
                {
                    await _videoApiClient.DeleteAudioApplicationAsync(hearingId);
                    _logger.LogInformation("Successfully deleted audio application with hearing id {hearingId}", hearingId);
                }
            }
            catch (VideoApiException e)
            {
                if (e.StatusCode != (int)HttpStatusCode.NotFound) return StatusCode(e.StatusCode, e.Response);
            }

            var response = new DeletedResponse()
            {
                NumberOfDeletedHearings = hearingIds.Count
            };

            return Ok(response);
        }
    }
}
