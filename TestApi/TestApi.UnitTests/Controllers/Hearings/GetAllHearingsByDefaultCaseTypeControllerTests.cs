using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;

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
                    ScheduledDate = DateTime.UtcNow
                }
            };

            const int LIMIT = HearingData.GET_HEARINGS_LIMIT;

            var bookingResponse = new BookingsResponse()
            {
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
            responses.Any(x => x.HearingName.Equals(bookingDetailsResponse.HearingName)).Should().BeTrue();
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
