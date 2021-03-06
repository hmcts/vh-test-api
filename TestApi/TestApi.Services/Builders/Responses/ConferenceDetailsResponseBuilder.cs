using System;
using System.Collections.Generic;
using TestApi.Common.Data;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace TestApi.Services.Builders.Responses
{
    public class ConferenceDetailsResponseBuilder
    {
        private readonly ConferenceDetailsResponse _response;
        private readonly BookNewConferenceRequest _request;

        public ConferenceDetailsResponseBuilder(BookNewConferenceRequest request)
        {
            _response = new ConferenceDetailsResponse();
            _request = request;
        }

        public ConferenceDetailsResponse BuildResponse()
        {
            _response.AudioRecordingRequired = _request.AudioRecordingRequired;
            _response.CaseName = _request.CaseName;
            _response.CaseNumber = _request.CaseNumber;
            _response.CaseType = _request.CaseType;
            _response.ClosedDateTime = null;
            _response.CurrentStatus = ConferenceState.NotStarted;
            _response.HearingId = _request.HearingRefId;
            _response.HearingVenueName = _request.HearingVenueName;
            _response.Id = Guid.NewGuid();

            _response.MeetingRoom = new MeetingRoomResponse
            {
                AdminUri = MeetingRoomData.MEETING_ROOM_ADMIN_URL,
                JudgeUri = MeetingRoomData.MEETING_ROOM_JUDGE_URL,
                ParticipantUri = MeetingRoomData.MEETING_ROOM_PARTICIPANT_URL,
                PexipNode = MeetingRoomData.MEETING_ROOM_PEXIP_NODE,
                PexipSelfTestNode = MeetingRoomData.MEETING_ROOM_PEXIP_SELF_TEST_NODE
            };

            _response.ScheduledDateTime = _request.ScheduledDateTime;
            _response.ScheduledDuration = _request.ScheduledDuration;
            _response.StartedDateTime = null;

            _response.Participants = AddParticipants();

            return _response;
        }

        private List<ParticipantDetailsResponse> AddParticipants()
        {
            var participants = new List<ParticipantDetailsResponse>();

            foreach (var participant in _request.Participants)
            {
                participants.Add(new ParticipantDetailsResponse()
                {
                    CaseTypeGroup = participant.CaseTypeGroup,
                    ContactEmail = participant.ContactEmail,
                    ContactTelephone = participant.ContactTelephone,
                    CurrentStatus = ParticipantState.NotSignedIn,
                    DisplayName = participant.DisplayName,
                    FirstName = participant.FirstName,
                    Id = Guid.NewGuid(),
                    LastName = participant.LastName,
                    Name = participant.Name,
                    RefId = participant.ParticipantRefId,
                    Representee = participant.Representee,
                    UserRole = participant.UserRole,
                    Username = participant.Username
                });
            }

            return participants;
        }
    }
}
