using System;
using System.Linq;
using TestApi.Common.Data;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

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
                ConferenceId = conference.Id.ToString(),
                EventId = Guid.NewGuid().ToString(),
                Reason = HearingData.VIDEO_EVENT_REASON,
                TimeStampUtc = DateTime.UtcNow
            };
        }

        public ConferenceEventRequestBuilder()
        {
            _request = new ConferenceEventRequest()
            {
                ConferenceId = Guid.NewGuid().ToString(),
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = Guid.NewGuid().ToString(),
                Reason = HearingData.VIDEO_EVENT_REASON,
                TimeStampUtc = DateTime.UtcNow
            };
        }

        public ConferenceEventRequestBuilder ForJudge()
        {
            _request.ParticipantId = _conference.Participants.Single(x => x.UserRole == UserRole.Judge).Id.ToString();
            return this;
        }

        public ConferenceEventRequestBuilder ForIndividual()
        {
            _request.ParticipantId = _conference.Participants.First(x => x.UserRole == UserRole.Individual).Id.ToString();
            return this;
        }

        public ConferenceEventRequestBuilder WithEventType(EventType eventType)
        {
            _request.EventType = eventType;
            return this;
        }

        public ConferenceEventRequest Build()
        {
            if (_request.EventType == EventType.Consultation)
            {
                _request.EventType = EventType.Transfer;
                _request.TransferTo = RoomData.ConsultationRoom;
            }

            if (_request.EventType != EventType.Transfer) return _request;
            _request.TransferFrom = RoomData.WaitingRoom;
            _request.TransferTo = RoomData.HearingRoom;

            return _request;
        }
    }
}
