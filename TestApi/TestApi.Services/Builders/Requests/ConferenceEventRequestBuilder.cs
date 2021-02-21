using System;
using System.Linq;
using TestApi.Common.Data;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.Services.Builders.Requests
{
    public class ConferenceEventRequestBuilder
    {
        private readonly ConferenceEventRequest _request;
        private readonly ConferenceDetailsResponse _conference;

        public ConferenceEventRequestBuilder(ConferenceDetailsResponse conference)
        {
            _conference = conference;
            _request = new ConferenceEventRequest()
            {
                Conference_id = conference.Id.ToString(),
                Event_id = Guid.NewGuid().ToString(),
                Reason = HearingData.VIDEO_EVENT_REASON,
                Time_stamp_utc = DateTime.UtcNow
            };
        }

        public ConferenceEventRequestBuilder()
        {
            _request = new ConferenceEventRequest()
            {
                Conference_id = Guid.NewGuid().ToString(),
                Event_id = Guid.NewGuid().ToString(),
                Participant_id = Guid.NewGuid().ToString(),
                Reason = HearingData.VIDEO_EVENT_REASON,
                Time_stamp_utc = DateTime.UtcNow
            };
        }

        public ConferenceEventRequestBuilder ForJudge()
        {
            _request.Participant_id = _conference.Participants.Single(x => x.User_role == UserRole.Judge).Id.ToString();
            return this;
        }

        public ConferenceEventRequestBuilder ForIndividual()
        {
            _request.Participant_id = _conference.Participants.First(x => x.User_role == UserRole.Individual).Id.ToString();
            return this;
        }

        public ConferenceEventRequestBuilder WithEventType(EventType eventType)
        {
            _request.Event_type = eventType;
            return this;
        }

        public ConferenceEventRequest Build()
        {
            if (_request.Event_type == EventType.Consultation)
            {
                _request.Event_type = EventType.Transfer;
                _request.Transfer_to = RoomData.ConsultationRoom;
            }

            if (_request.Event_type != EventType.Transfer) return _request;
            _request.Transfer_from = RoomData.WaitingRoom;
            _request.Transfer_to = RoomData.HearingRoom;

            return _request;
        }
    }
}
