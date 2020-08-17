using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class DeleteHearingControllerTests : HearingsControllerTestBase
    {
        [Test]
        public async Task Should_delete_hearing()
        {
            var hearingId = Guid.NewGuid();
            var hearingDetailsResponse = GetHearingDetailsResponse();

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(hearingDetailsResponse);

            BookingsApiClient
                .Setup(x => x.RemoveHearingAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            var response = await Controller.DeleteHearingByIdAsync(hearingId);
            response.Should().NotBeNull();

            var result = (NoContentResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NoContent);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing_id()
        {
            var hearingId = Guid.NewGuid();
            var hearingDetailsResponse = GetHearingDetailsResponse();

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(hearingDetailsResponse);

            BookingsApiClient
                .Setup(x => x.RemoveHearingAsync(It.IsAny<Guid>()))
                .ThrowsAsync(GetBookingsApiException("Hearing not found", HttpStatusCode.NotFound));

            var response = await Controller.DeleteHearingByIdAsync(hearingId);
            response.Should().NotBeNull();

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }
    }
}