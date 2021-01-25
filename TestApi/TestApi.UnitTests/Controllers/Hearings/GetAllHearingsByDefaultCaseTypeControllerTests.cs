using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class GetAllHearingsByDefaultCaseTypeControllerTests : HearingsControllerTestsBase
    {
        [Test]
        public async Task Should_return_all_hearings_by_default_case_type()
        {
            var bookingDetailsResponse = CreateBookingDetailsResponse();
            var bookingDetailsResponses = new List<BookingsHearingResponse> {bookingDetailsResponse};
            var bookingsByDateResponses = new List<BookingsByDateResponse>()
            {
                new BookingsByDateResponse()
                {
                    Hearings = bookingDetailsResponses,
                    Scheduled_date = DateTime.UtcNow
                }
            };

            const int LIMIT = HearingData.GET_HEARINGS_LIMIT;

            var bookingResponse = new BookingsResponse()
            {
                AdditionalProperties = new Dictionary<string, object>(),
                Hearings = bookingsByDateResponses,
                Limit = LIMIT
            };

            BookingsApiClient
                .Setup(
                    x => x.GetHearingsByTypesAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(bookingResponse);

            var response = await Controller.GetAllHearingsAsync();
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var responses = (List<BookingsHearingResponse>) result.Value;
            responses.Should().NotBeEmpty();
            responses.Any(x => x.Hearing_name.Equals(bookingDetailsResponse.Hearing_name)).Should().BeTrue();
        }

        [Test]
        public async Task Should_throw_error_for_all_hearings_by_default_case_type()
        {
            BookingsApiClient
                .Setup(
                    x => x.GetHearingsByTypesAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<int>()))
                .ThrowsAsync(CreateBookingsApiException("Failed", HttpStatusCode.InternalServerError));

            var response = await Controller.GetAllHearingsAsync();
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task Should_return_okay_if_no_hearings_found()
        {
            BookingsApiClient
                .Setup(
                    x => x.GetHearingsByTypesAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<int>()))
                .ThrowsAsync(CreateBookingsApiException("Failed", HttpStatusCode.NotFound));

            var response = await Controller.GetAllHearingsAsync();
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var responses = (List<BookingsHearingResponse>)result.Value;
            responses.Should().BeEmpty();
        }
    }
}
