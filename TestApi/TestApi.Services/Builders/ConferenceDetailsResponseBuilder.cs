using System;
using System.Linq;
using TestApi.Common.Data;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.Services.Builders
{
    public class ConferenceDetailsResponseBuilder
    {
        private readonly HearingDetailsResponse _hearingDetails;

        public ConferenceDetailsResponseBuilder(HearingDetailsResponse hearingDetails)
        {
            _hearingDetails = hearingDetails;
        }

        public ConferenceDetailsResponse Build()
        {
            var participants = _hearingDetails.Participants.Select(participant => new ParticipantDetailsResponse
                {
                    Case_type_group = participant.Case_role_name,
                    Current_status = ParticipantState.NotSignedIn,
                    Display_name = participant.Display_name,
                    First_name = participant.First_name,
                    Id = Guid.NewGuid(),
                    Last_name = participant.Last_name,
                    Name = $"{participant.Title} {participant.First_name} {participant.First_name}",
                    Ref_id = participant.Id,
                    Representee = participant.Representee,
                    User_role = GetUserRole(participant.Last_name),
                    Username = participant.Username
                })
                .ToList();

            var meetingRoom = new MeetingRoomResponse
            {
                Admin_uri = DefaultData.MEETING_ROOM_ADMIN_URL,
                Judge_uri = DefaultData.MEETING_ROOM_JUDGE_URL,
                Participant_uri = DefaultData.MEETING_ROOM_PARTICIPANT_URL,
                Pexip_node = DefaultData.MEETING_ROOM_PEXIP_NODE,
                Pexip_self_test_node = DefaultData.MEETING_ROOM_PEXIP_SELF_TEST_NODE
            };

            return new ConferenceDetailsResponse
            {
                Audio_recording_required = _hearingDetails.Audio_recording_required,
                Case_name = _hearingDetails.Cases.First().Name,
                Case_number = _hearingDetails.Cases.First().Number,
                Case_type = _hearingDetails.Case_type_name,
                Closed_date_time = null,
                Current_status = ConferenceState.NotStarted,
                Hearing_id = _hearingDetails.Id,
                Hearing_venue_name = _hearingDetails.Hearing_venue_name,
                Id = Guid.NewGuid(),
                Meeting_room = meetingRoom,
                Participants = participants,
                Scheduled_date_time = _hearingDetails.Scheduled_date_time,
                Scheduled_duration = _hearingDetails.Scheduled_duration,
                Started_date_time = null
            };
        }

        private UserRole GetUserRole(string lastName)
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