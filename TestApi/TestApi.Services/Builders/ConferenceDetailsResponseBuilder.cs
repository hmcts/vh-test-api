using System;
using System.Collections.Generic;
using System.Linq;
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
            var participants = new List<ParticipantDetailsResponse>();

            foreach (var participant in _hearingDetails.Participants)
                participants.Add(new ParticipantDetailsResponse
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
                });

            var meetingRoom = new MeetingRoomResponse
            {
                Admin_uri = "url",
                Judge_uri = "url",
                Participant_uri = "url",
                Pexip_node = "node",
                Pexip_self_test_node = "node"
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