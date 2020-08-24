using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class GetConferencesForJudgesTests : ConferencesTestsBase
    {
        [Test]
        public async Task Should_get_conferences_for_a_judge()
        {
            var request = CreateConferenceRequest();
            var conference = await CreateConference(request);
            var judge = conference.Participants.First(x => x.User_role == UserRole.Judge);

            var uri = ApiUriFactory.ConferenceEndpoints.GetConferencesForJudge(judge.Username);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<ConferenceForJudgeResponse>>(Json);

            response.Should().NotBeNull();
            Verify.ConferencesForJudgeResponses(response, request);
        }

        [Test]
        public async Task Should_return_empty_list_for_non_existent_username()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;
            var uri = ApiUriFactory.ConferenceEndpoints.GetConferencesForJudge(USERNAME);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<ConferenceForJudgeResponse>>(Json);
            response.Should().NotBeNull();
            response.Count.Should().Be(0);
        }
    }
}
