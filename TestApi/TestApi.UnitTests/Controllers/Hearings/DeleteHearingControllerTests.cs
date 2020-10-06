using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class DeleteHearingControllerTests : HearingsControllerTestsBase
    {
        [Test]
        public async Task Should_delete_hearing()
        {
            var hearingId = Guid.NewGuid();
            var hearingDetailsResponse = CreateHearingDetailsResponse();

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(hearingDetailsResponse);

            BookingsApiClient
                .Setup(x => x.RemoveHearingAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            VideoApiClient
                .Setup(x => x.DeleteAudioApplicationAsync(It.IsAny<Guid>()))
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
            var hearingDetailsResponse = CreateHearingDetailsResponse();

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(hearingDetailsResponse);

            BookingsApiClient
                .Setup(x => x.RemoveHearingAsync(It.IsAny<Guid>()))
                .ThrowsAsync(CreateBookingsApiException("Hearing not found", HttpStatusCode.NotFound));

            var response = await Controller.DeleteHearingByIdAsync(hearingId);
            response.Should().NotBeNull();

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_delete_hearing_without_removing_audio_application_with_no_audio_application()
        {
            var hearingId = Guid.NewGuid();
            var hearingDetailsResponse = CreateHearingDetailsResponse();

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(hearingDetailsResponse);

            BookingsApiClient
                .Setup(x => x.RemoveHearingAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            VideoApiClient
                .Setup(x => x.DeleteAudioApplicationAsync(It.IsAny<Guid>()))
                .ThrowsAsync(CreateVideoApiException("No audio application found", HttpStatusCode.NotFound));

            var response = await Controller.DeleteHearingByIdAsync(hearingId);
            response.Should().NotBeNull();

            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }
}