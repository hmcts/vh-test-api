using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using TestApi.Tests.Common.Configuration;
using VideoApi.Contract.Enums;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class DeleteParticipantTests : ConferencesTestsBase
    {
        [Test]
        public async Task Should_delete_participant_by_id()
        {
            var request = CreateConferenceRequest();
            var conference = await CreateConference(request);
            var participant = conference.Participants.First(x => x.UserRole == UserRole.Individual);
            var uri = ApiUriFactory.ConferenceEndpoints.DeleteParticipant(conference.Id, participant.Id);
            
            await SendDeleteRequest(uri);

            VerifyResponse(HttpStatusCode.NoContent, true);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_conference_id()
        {
            var request = CreateConferenceRequest();
            var conference = await CreateConference(request);
            var participant = conference.Participants.First(x => x.UserRole == UserRole.Individual);

            var uri = ApiUriFactory.ConferenceEndpoints.DeleteParticipant(Guid.NewGuid(), participant.Id);
            await SendDeleteRequest(uri);
            VerifyResponse(HttpStatusCode.NotFound, false);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_participant_id()
        {
            var request = CreateConferenceRequest();
            var conference = await CreateConference(request);

            var uri = ApiUriFactory.ConferenceEndpoints.DeleteParticipant(conference.Id, Guid.NewGuid());
            await SendDeleteRequest(uri);
            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
