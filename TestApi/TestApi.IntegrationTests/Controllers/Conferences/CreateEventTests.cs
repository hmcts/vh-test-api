using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class CreateEventTests : ConferencesTestsBase
    {
        [Test]
        public async Task Should_create_video_event()
        {
            var conferenceRequest = CreateConferenceRequest();
            var conference = await CreateConference(conferenceRequest);
            var videoEventRequest = CreateVideoEventRequest(conference);
            await CreateEvent(videoEventRequest);

            VerifyResponse(HttpStatusCode.NoContent, true);
        }

        [Test]
        public async Task Should_not_create_video_event_with_invalid_request()
        {
            var conferenceRequest = CreateConferenceRequest();
            var conference = await CreateConference(conferenceRequest);

            const EventType EVENT_TYPE = EventType.None;
            const int EVENT_TYPE_ID = (int)EVENT_TYPE;

            var participant = conference.Participants.First(x => x.User_role == UserRole.Individual);

            var request = new ConferenceEventRequest()
            {
                Conference_id = conference.Id.ToString(),
                Event_id = EVENT_TYPE_ID.ToString(),
                Event_type = EVENT_TYPE,
                Participant_id = participant.Id.ToString(),
                Phone = string.Empty,
                Reason = HearingData.VIDEO_EVENT_REASON,
                Time_stamp_utc = DateTime.UtcNow,
                Transfer_from = null,
                Transfer_to = null
            };

            var uri = ApiUriFactory.ConferenceEndpoints.CreateVideoEvent;
            await SendPostRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.BadRequest, false);
        }

        [Test]
        public async Task Should_create_transfer_event_with_room_types()
        {
            var conferenceRequest = CreateConferenceRequest();
            var conference = await CreateConference(conferenceRequest);
            var videoEventRequest = CreateTransferEventRequest(conference);
            await CreateEvent(videoEventRequest);

            VerifyResponse(HttpStatusCode.NoContent, true);
        }

        [Test]
        public async Task Should_create_private_consultation_event()
        {
            var conferenceRequest = CreateConferenceRequest();
            var conference = await CreateConference(conferenceRequest);
            var videoEventRequest = CreatePrivateConsultationEventRequest(conference);
            await CreateEvent(videoEventRequest);

            VerifyResponse(HttpStatusCode.NoContent, true);
        }
    }
}
