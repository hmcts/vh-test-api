using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Responses;

namespace TestApi.UnitTests.Controllers.Conferences
{
    public class GetConferenceByIdControllerTests : ConferencesControllerTestsBase
    {
        [Test]
        public async Task Should_get_conference_by_conference_id()
        {
            var conferenceDetailsResponse = CreateConferenceDetailsResponse();

            VideoApiClient
                .Setup(x => x.GetConferenceDetailsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(conferenceDetailsResponse);

            var response = await Controller.GetConferenceByIdAsync(conferenceDetailsResponse.Id);
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var conferenceDetails = (ConferenceDetailsResponse)result.Value;
            conferenceDetails.Should().NotBeNull();
            conferenceDetails.Should().BeEquivalentTo(conferenceDetailsResponse);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_conference_id()
        {
            VideoApiClient
                .Setup(x => x.GetConferenceDetailsByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(CreateVideoApiException("Conference not found", HttpStatusCode.NotFound));

            var response = await Controller.GetConferenceByIdAsync(Guid.NewGuid());
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
