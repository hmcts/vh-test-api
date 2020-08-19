using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class GetConferencesForVhoTests : ConferencesTestsBase
    {
        [Test]
        public async Task Should_get_conferences_for_a_vho()
        {
            var request = CreateConferenceRequest();
            var conference = await CreateConference(request);
            var judge = conference.Participants.First(x => x.User_role == UserRole.Judge);
            var usernames = new[] {judge.Username};

            var uri = ApiUriFactory.ConferenceEndpoints.GetConferencesForVho(usernames);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<ConferenceForAdminResponse>>(Json);

            response.Should().NotBeNull();
            Verify.ConferencesForVhoResponses(response, request);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_username()
        {
            var usernames = new [] {"made_up_username@email.com"};
            var uri = ApiUriFactory.ConferenceEndpoints.GetConferencesForVho(usernames);
            await SendGetRequest(uri);
            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
