using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Tests.Common.Configuration;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;

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

            var participant = conference.Participants.First(x => x.UserRole == UserRole.Individual);

            var request = new ConferenceEventRequest()
            {
                ConferenceId = conference.Id.ToString(),
                EventId = EVENT_TYPE_ID.ToString(),
                EventType = EVENT_TYPE,
                ParticipantId = participant.Id.ToString(),
                Phone = string.Empty,
                Reason = HearingData.VIDEO_EVENT_REASON,
                TimeStampUtc = DateTime.UtcNow,
                TransferFrom = null,
                TransferTo = null
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
