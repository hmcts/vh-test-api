using System.Collections.Generic;
using System.Linq;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.Services.Mappings
{
    public static class ConferenceRequestToAdminTodayMapper
    {
        public static List<ConferenceForAdminResponse> Map(ConferenceDetailsResponse response)
        {
            return new List<ConferenceForAdminResponse>()
            {
                new ConferenceForAdminResponse()
                {
                    Case_name = response.Case_name,
                    Case_number = response.Case_number,
                    Case_type = response.Case_type,
                    Closed_date_time =response.Closed_date_time,
                    Hearing_venue_name = response.Hearing_venue_name,
                    Hearing_ref_id = response.Hearing_id,
                    Id = response.Id,
                    Participants = AddParticipants(response.Participants),
                    Scheduled_date_time = response.Scheduled_date_time,
                    Scheduled_duration = response.Scheduled_duration,
                    Started_date_time = response.Started_date_time,
                    Status = response.Current_status
                }
            };
        }

        private static List<ParticipantSummaryResponse> AddParticipants(IEnumerable<ParticipantDetailsResponse> requestParticipants)
        {
            return requestParticipants.Select(participant => new ParticipantSummaryResponse()
                {
                    Case_group = participant.Case_type_group,
                    Contact_email = participant.Contact_email,
                    Contact_telephone = participant.Contact_telephone,
                    Display_name = participant.Display_name,
                    First_name = participant.First_name,
                    Id = participant.Id,
                    Last_name = participant.Last_name,
                    Representee = participant.Representee,
                    Status = participant.Current_status,
                    User_role = participant.User_role,
                    Username = participant.Username
                })
                .ToList();
        }
    }
}
