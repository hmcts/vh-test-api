using System;
using System.Linq;
using BookingsApi.Contract.Responses;
using TestApi.Common.Data;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;

namespace TestApi.Services.Builders.Responses
{
    public class ConferenceDetailsFromHearingResponseBuilder
    {
        private readonly HearingDetailsResponse _hearingDetails;

        public ConferenceDetailsFromHearingResponseBuilder(HearingDetailsResponse hearingDetails)
        {
            _hearingDetails = hearingDetails;
        }

        public ConferenceDetailsResponse Build()
        {
            var participants = _hearingDetails.Participants.Select(participant => new ParticipantDetailsResponse
                {
                    CaseTypeGroup = participant.CaseRoleName,
                    CurrentStatus = ParticipantState.NotSignedIn,
                    DisplayName = participant.DisplayName,
                    FirstName = participant.FirstName,
                    Id = Guid.NewGuid(),
                    LastName = participant.LastName,
                    Name = $"{participant.Title} {participant.FirstName} {participant.FirstName}",
                    RefId = participant.Id,
                    Representee = participant.Representee,
                    UserRole = GetUserRole(participant.LastName),
                    Username = participant.Username
                })
                .ToList();

            var meetingRoom = new MeetingRoomResponse
            {
                AdminUri = MeetingRoomData.MEETING_ROOM_ADMIN_URL,
                JudgeUri = MeetingRoomData.MEETING_ROOM_JUDGE_URL,
                ParticipantUri = MeetingRoomData.MEETING_ROOM_PARTICIPANT_URL,
                PexipNode = MeetingRoomData.MEETING_ROOM_PEXIP_NODE,
                PexipSelfTestNode = MeetingRoomData.MEETING_ROOM_PEXIP_SELF_TEST_NODE
            };

            return new ConferenceDetailsResponse
            {
                AudioRecordingRequired = _hearingDetails.AudioRecordingRequired,
                CaseName = _hearingDetails.Cases.First().Name,
                CaseNumber = _hearingDetails.Cases.First().Number,
                CaseType = _hearingDetails.CaseTypeName,
                ClosedDateTime = null,
                CurrentStatus = ConferenceState.NotStarted,
                HearingId = _hearingDetails.Id,
                HearingVenueName = _hearingDetails.HearingVenueName,
                Id = Guid.NewGuid(),
                MeetingRoom = meetingRoom,
                Participants = participants,
                ScheduledDateTime = _hearingDetails.ScheduledDateTime,
                ScheduledDuration = _hearingDetails.ScheduledDuration,
                StartedDateTime = null
            };
        }

        private static UserRole GetUserRole(string lastName)
        {
            lastName = RemoveUnderscores(lastName);
            lastName = RemoveNumbers(lastName);
            Enum.TryParse(lastName, true, out UserRole userRole);
            return userRole;
        }

        private static string RemoveUnderscores(string text)
        {
            return text.Replace("_", string.Empty).Trim();
        }

        private static string RemoveNumbers(string text)
        {
            return new string(text.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
        }
    }
}