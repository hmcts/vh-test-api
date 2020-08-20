using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Services.Mappings;

namespace TestApi.UnitTests.Controllers.Conferences
{
    public class GetConferencesForTodayJudgeControllerTests : ConferencesControllerTestsBase
    {
        [Test]
        public async Task Should_get_conferences_for_today_judge()
        {
            var conferenceDetailsResponse = CreateConferenceDetailsResponse();
            var judge = conferenceDetailsResponse.Participants.First(x => x.User_role == UserRole.Judge);
            var judgeResponse = ConferenceRequestToJudgeTodayMapper.Map(conferenceDetailsResponse);

            VideoApiClient
                .Setup(x => x.GetConferencesTodayForJudgeByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(judgeResponse);

            var response = await Controller.GetConferencesForTodayJudgeAsync(judge.Username);
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var conferenceDetails = (List<ConferenceForJudgeResponse>)result.Value;
            conferenceDetails.Should().NotBeNull();
            conferenceDetails.Should().BeEquivalentTo(judgeResponse);
        }

        [Test]
        public async Task Should_return_not_found_if_Judge_does_not_exist()
        {
            VideoApiClient
                .Setup(x => x.GetConferencesTodayForJudgeByUsernameAsync(It.IsAny<string>()))
                .ThrowsAsync(CreateVideoApiException("Judge does not exist", HttpStatusCode.NotFound));

            var response = await Controller.GetConferencesForTodayJudgeAsync("made_up_email@email.com");
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
