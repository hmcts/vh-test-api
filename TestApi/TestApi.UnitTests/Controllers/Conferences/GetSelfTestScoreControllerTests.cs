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
    public class GetSelfTestScoreControllerTests : ConferencesControllerTestsBase
    {
        [Test]
        public async Task Should_get_self_test_score()
        {
            var selfTestResponse = new TestCallScoreResponse()
            {
                Passed = true,
                Score = TestScore.Good
            };

            VideoApiClient
                .Setup(x => x.GetTestCallResultForParticipantAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(selfTestResponse);

            var response = await Controller.GetSelfTestScoreAsync(Guid.NewGuid(), Guid.NewGuid());
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var conferenceDetails = (TestCallScoreResponse)result.Value;
            conferenceDetails.Should().NotBeNull();
            conferenceDetails.Should().BeEquivalentTo(selfTestResponse);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_conference_and_participant_ids()
        {
            VideoApiClient
                .Setup(x => x.GetTestCallResultForParticipantAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(CreateVideoApiException("Conference not found", HttpStatusCode.NotFound));

            var response = await Controller.GetSelfTestScoreAsync(Guid.NewGuid(), Guid.NewGuid());
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
