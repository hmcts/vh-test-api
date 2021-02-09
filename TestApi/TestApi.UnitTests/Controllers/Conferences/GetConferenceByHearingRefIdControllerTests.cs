using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.UnitTests.Controllers.Conferences
{
    public class GetConferenceByHearingRefIdControllerTests : ConferencesControllerTestsBase
    {
        [Test]
        public async Task Should_get_conference_by_hearing_ref_id()
        {
            var conferenceDetailsResponse = CreateConferenceDetailsResponse();

            VideoApiClient
                .Setup(x => x.GetConferenceByHearingRefIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(conferenceDetailsResponse);

            var response = await Controller.GetConferenceByHearingRefIdAsync(conferenceDetailsResponse.Hearing_id);
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var conferenceDetails = (ConferenceDetailsResponse)result.Value;
            conferenceDetails.Should().NotBeNull();
            conferenceDetails.Should().BeEquivalentTo(conferenceDetailsResponse);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing_ref_id()
        {
            VideoApiClient
                .Setup(x => x.GetConferenceByHearingRefIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ThrowsAsync(CreateVideoApiException("Conference not found", HttpStatusCode.NotFound));

            var response = await Controller.GetConferenceByHearingRefIdAsync(Guid.NewGuid());
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
