using System.Collections.Generic;
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
            var vhoResponse = ConferenceRequestToAdminTodayMapper.Map(conferenceDetailsResponse);

            VideoApiClient
                .Setup(x => x.GetConferencesTodayForAdminAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(vhoResponse);

            var response = await Controller.GetConferencesForTodayVhoAsync();
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var conferenceDetails = (List<ConferenceForAdminResponse>)result.Value;
            conferenceDetails.Should().NotBeNull();
            conferenceDetails.Should().BeEquivalentTo(vhoResponse);
        }
    }
}
