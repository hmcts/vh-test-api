using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class GetHearingByIdControllerTests : HearingsControllerTestBase
    {
        [Test]
        public async Task Should_return_hearing()
        {
            var hearingDetailsResponse = GetHearingDetailsResponse();

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(hearingDetailsResponse.Id))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.GetHearingByIdAsync(hearingDetailsResponse.Id);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var hearingDetails = (HearingDetailsResponse) result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing()
        {
            var nonExistentHearingId = Guid.NewGuid();

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(nonExistentHearingId))
                .ThrowsAsync(GetBookingsApiException("Hearing not found", HttpStatusCode.NotFound));

            var response = await Controller.GetHearingByIdAsync(nonExistentHearingId);
            response.Should().NotBeNull();

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }
    }
}