using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class ConfirmHearingsControllerTests : HearingsControllerTestBase
    {
        [Test]
        public async Task Should_confirm_hearing()
        {
            var hearingId = Guid.NewGuid();
            var conferenceDetailsResponse = GetConferenceDetailsResponse();

            var request = new UpdateBookingStatusRequest()
            {
                AdditionalProperties = null,
                Cancel_reason = null,
                Status = UpdateBookingStatus.Created,
                Updated_by = "updated_by@email.com"
            };

            BookingsApiClient
                .Setup(x => x.UpdateBookingStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateBookingStatusRequest>()))
                .Returns(Task.CompletedTask);

            VideoApiService
                .Setup(x => x.GetConferenceByIdPollingAsync(It.IsAny<Guid>()))
                .ReturnsAsync(conferenceDetailsResponse);

            var response = await Controller.ConfirmHearingByIdAsync(hearingId, request);
            response.Should().NotBeNull();

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);

            var conferenceDetails = (ConferenceDetailsResponse) result.Value;
            conferenceDetails.Should().BeEquivalentTo(conferenceDetailsResponse);
        }

        [Test]
        public async Task Should_throw_not_found_for_non_existent_hearing_id()
        {
            var hearingId = Guid.NewGuid();
            var request = new UpdateBookingStatusRequest()
            {
                AdditionalProperties = null,
                Cancel_reason = null,
                Status = UpdateBookingStatus.Created,
                Updated_by = "updated_by@email.com"
            };

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(It.IsAny<Guid>()))
                .Throws(GetBookingsApiException("Hearing not found", HttpStatusCode.NotFound));

            var response = await Controller.ConfirmHearingByIdAsync(hearingId, request);
            response.Should().NotBeNull();

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_throw_not_found_if_conference_not_created()
        {
            var hearingId = Guid.NewGuid();
            var request = new UpdateBookingStatusRequest()
            {
                AdditionalProperties = null,
                Cancel_reason = null,
                Status = UpdateBookingStatus.Created,
                Updated_by = "updated_by@email.com"
            };

            BookingsApiService
                .Setup(x => x.UpdateBookingStatusPollingAsync(It.IsAny<Guid>(), It.IsAny<UpdateBookingStatusRequest>()))
                .Returns(Task.CompletedTask);

            VideoApiService
                .Setup(x => x.GetConferenceByIdPollingAsync(It.IsAny<Guid>()))
                .ThrowsAsync(GetVideoApiException("Conference not found", HttpStatusCode.NotFound));

            var response = await Controller.ConfirmHearingByIdAsync(hearingId, request);
            response.Should().NotBeNull();

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }
    }
}