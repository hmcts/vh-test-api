using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class DeleteConferenceTests : ConferencesTestsBase
    {
        [Test]
        public async Task Should_delete_conference_by_id()
        {
            var request = CreateConferenceRequest();
            var conference = await CreateConference(request);

            await DeleteConference(conference.Id);

            VerifyResponse(HttpStatusCode.NoContent, true);
            ConferencesToDelete.Remove(conference);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_conference_id()
        {
            var uri = ApiUriFactory.ConferenceEndpoints.DeleteConference(Guid.NewGuid());
            await SendGetRequest(uri);
            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
