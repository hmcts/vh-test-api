using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using TestApi.Tests.Common.Configuration;
using VideoApi.Contract.Enums;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class GetSelfTestScoreTests : ConferencesTestsBase
    {
        [Test]
        public async Task Should_get_not_found_for_non_existent_self_test_score()
        {
            var request = CreateConferenceRequest();
            var conference = await CreateConference(request);
            var participant = conference.Participants.First(x => x.UserRole == UserRole.Individual);

            var uri = ApiUriFactory.ConferenceEndpoints.GetSelfTestScore(conference.Id, participant.Id);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
