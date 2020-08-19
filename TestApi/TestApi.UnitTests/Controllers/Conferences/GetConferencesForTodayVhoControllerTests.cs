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
    public class GetConferencesForTodayVhoControllerTests : ConferencesControllerTestsBase
    {
        [Test]
        public async Task Should_get_conferences_for_today_vho()
        {
            var conferenceDetailsResponse = CreateConferenceDetailsResponse();
            var judge = conferenceDetailsResponse.Participants.First(x => x.User_role == UserRole.Judge);
            var vhoResponse = ConferenceRequestToAdminTodayMapper.Map(conferenceDetailsResponse);
            var usernames = new[] {judge.Username};

            VideoApiClient
                .Setup(x => x.GetConferencesTodayForAdminAsync(It.IsAny<string[]>()))
                .ReturnsAsync(vhoResponse);

            var response = await Controller.GetConferencesForTodayVhoAsync(usernames);
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var conferenceDetails = (List<ConferenceForAdminResponse>)result.Value;
            conferenceDetails.Should().NotBeNull();
            conferenceDetails.Should().BeEquivalentTo(vhoResponse);
        }

        [Test]
        public async Task Should_return_not_found_if_Judge_does_not_exist()
        {
            var usernames = new[] { "made_up_email@email.com" };

            VideoApiClient
                .Setup(x => x.GetConferencesTodayForAdminAsync(It.IsAny<string[]>()))
                .ThrowsAsync(CreateVideoApiException("Username does not exist", HttpStatusCode.NotFound));

            var response = await Controller.GetConferencesForTodayVhoAsync(usernames);
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
