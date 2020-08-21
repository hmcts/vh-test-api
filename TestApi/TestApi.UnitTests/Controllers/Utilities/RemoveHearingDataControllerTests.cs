using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Contract.Requests;

namespace TestApi.UnitTests.Controllers.Utilities
{
    public class RemoveHearingDataControllerTests : UtilitiesControllerTestsBase
    {
        [Test]
        public async Task Should_delete_hearings()
        {
            var request = new DeleteTestHearingDataRequest()
            {
                PartialHearingCaseName = "Test",
                PartialHearingCaseNumber = string.Empty
            };

            var idsResponse = new List<Guid>(){Guid.NewGuid()};

            BookingsApiService
                .Setup(x => x.DeleteHearingsByPartialCaseTextAsync(It.IsAny<DeleteTestHearingDataRequest>()))
                .ReturnsAsync(idsResponse);

            VideoApiClient
                .Setup(x => x.DeleteAudioApplicationAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            var response = await Controller.DeleteTestDataByPartialCaseTextAsync(request);

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var hearingDetails = (int)result.Value;
            hearingDetails.Should().Be(1);
        }

        [Test]
        public async Task Should_return_bad_request_with_invalid_request()
        {
            BookingsApiService
                .Setup(x => x.DeleteHearingsByPartialCaseTextAsync(It.IsAny<DeleteTestHearingDataRequest>()))
                .ThrowsAsync(CreateBookingsApiException("Request invalid", HttpStatusCode.BadRequest));
            
            var response = await Controller.DeleteTestDataByPartialCaseTextAsync(new DeleteTestHearingDataRequest());

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Should_return_bad_request_with_video_api_internal_error()
        {
            var idsResponse = new List<Guid>() { Guid.NewGuid() };

            BookingsApiService
                .Setup(x => x.DeleteHearingsByPartialCaseTextAsync(It.IsAny<DeleteTestHearingDataRequest>()))
                .ReturnsAsync(idsResponse);

            VideoApiClient
                .Setup(x => x.DeleteAudioApplicationAsync(It.IsAny<Guid>()))
                .ThrowsAsync(CreateVideoApiException("Request failed", HttpStatusCode.BadRequest));

            var response = await Controller.DeleteTestDataByPartialCaseTextAsync(new DeleteTestHearingDataRequest());

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}
