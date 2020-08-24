using System;
using System.Collections.Generic;
using TestApi.Common.Data;
using TestApi.Services.Clients.VideoApiClient;

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
            _response.Audio_recording_required = _request.Audio_recording_required;
            _response.Case_name = _request.Case_name;
            _response.Case_number = _request.Case_number;
            _response.Case_type = _request.Case_type;
            _response.Closed_date_time = null;
            _response.Current_status = ConferenceState.NotStarted;
            _response.Hearing_id = _request.Hearing_ref_id;
            _response.Hearing_venue_name = _request.Hearing_venue_name;
            _response.Id = Guid.NewGuid();

            _response.Meeting_room = new MeetingRoomResponse
            {
                Admin_uri = MeetingRoomData.MEETING_ROOM_ADMIN_URL,
                Judge_uri = MeetingRoomData.MEETING_ROOM_JUDGE_URL,
                Participant_uri = MeetingRoomData.MEETING_ROOM_PARTICIPANT_URL,
                Pexip_node = MeetingRoomData.MEETING_ROOM_PEXIP_NODE,
                Pexip_self_test_node = MeetingRoomData.MEETING_ROOM_PEXIP_SELF_TEST_NODE
            };

            _response.Scheduled_date_time = _request.Scheduled_date_time;
            _response.Scheduled_duration = _request.Scheduled_duration;
            _response.Started_date_time = null;

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
                    Case_type_group = participant.Case_type_group,
                    Contact_email = participant.Contact_email,
                    Contact_telephone = participant.Contact_telephone,
                    Current_status = ParticipantState.NotSignedIn,
                    Display_name = participant.Display_name,
                    First_name = participant.First_name,
                    Id = Guid.NewGuid(),
                    Last_name = participant.Last_name,
                    Name = participant.Name,
                    Ref_id = participant.Participant_ref_id,
                    Representee = participant.Representee,
                    User_role = participant.User_role,
                    Username = participant.Username
                });
            }

            return participants;
        }
    }
}
