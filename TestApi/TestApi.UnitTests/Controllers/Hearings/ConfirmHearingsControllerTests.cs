using System;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Builders.Requests;
using VideoApi.Contract.Responses;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class ConfirmHearingsControllerTests : HearingsControllerTestsBase
    {
        [Test]
        public async Task Should_confirm_hearing()
        {
            var hearingId = Guid.NewGuid();
            var conferenceDetailsResponse = CreateConferenceDetailsResponse();

            const string UPDATED_BY = UserData.DEFAULT_UPDATED_BY_USER;

            var request = new UpdateBookingRequestBuilder().UpdatedBy(UPDATED_BY).Build();

            BookingsApiClient
                .Setup(x => x.UpdateBookingStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateBookingStatusRequest>()))
                .Returns(Task.CompletedTask);

            VideoApiService
                .Setup(x => x.GetConferenceByHearingIdPollingAsync(It.IsAny<Guid>()))
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
            const string UPDATED_BY = UserData.DEFAULT_UPDATED_BY_USER;

            var request = new UpdateBookingRequestBuilder().UpdatedBy(UPDATED_BY).Build();

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(It.IsAny<Guid>()))
                .Throws(CreateBookingsApiException("Hearing not found", HttpStatusCode.NotFound));

            var response = await Controller.ConfirmHearingByIdAsync(hearingId, request);
            response.Should().NotBeNull();

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_throw_not_found_if_conference_not_created()
        {
            var hearingId = Guid.NewGuid();
            const string UPDATED_BY = UserData.DEFAULT_UPDATED_BY_USER;

            var request = new UpdateBookingRequestBuilder().UpdatedBy(UPDATED_BY).Build();

            BookingsApiService
                .Setup(x => x.UpdateBookingStatusPollingAsync(It.IsAny<Guid>(), It.IsAny<UpdateBookingStatusRequest>()))
                .Returns(Task.CompletedTask);

            VideoApiService
                .Setup(x => x.GetConferenceByHearingIdPollingAsync(It.IsAny<Guid>()))
                .ThrowsAsync(CreateVideoApiException("Conference not found", HttpStatusCode.NotFound));

            var response = await Controller.ConfirmHearingByIdAsync(hearingId, request);
            response.Should().NotBeNull();

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_throw_error_if_update_booking_status_failed()
        {
            var hearingId = Guid.NewGuid();
            const string UPDATED_BY = UserData.DEFAULT_UPDATED_BY_USER;

            var conferenceDetailsResponse = CreateConferenceDetailsResponse();

            var request = new UpdateBookingRequestBuilder().UpdatedBy(UPDATED_BY).Build();

            VideoApiService
                .Setup(x => x.GetConferenceByHearingIdPollingAsync(It.IsAny<Guid>()))
                .ReturnsAsync(conferenceDetailsResponse);

            BookingsApiService
                .Setup(x => x.UpdateBookingStatusPollingAsync(It.IsAny<Guid>(), It.IsAny<UpdateBookingStatusRequest>()))
                .ThrowsAsync(CreateBookingsApiException("Hearing not found", HttpStatusCode.NotFound));

            var response = await Controller.ConfirmHearingByIdAsync(hearingId, request);
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}