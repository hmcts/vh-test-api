using System;
using System.Collections.Generic;
using System.Linq;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.Services.Mappings
{
    public static class ConferenceRequestToJudgeTodayMapper
    {
        public static List<ConferenceForJudgeResponse> Map(ConferenceDetailsResponse response)
        {
            return new List<ConferenceForJudgeResponse>()
            {
                new ConferenceForJudgeResponse()
                {
                    Case_name = response.Case_name,
                    Case_number = response.Case_number,
                    Case_type = response.Case_type,
                    Id = Guid.NewGuid(),
                    Participants = AddParticipants(response.Participants),
                    Scheduled_date_time = response.Scheduled_date_time,
                    Scheduled_duration = response.Scheduled_duration,
                    Status = ConferenceState.NotStarted
                }
            };
        }

        private static List<ParticipantForJudgeResponse> AddParticipants(IEnumerable<ParticipantDetailsResponse> requestParticipants)
        {
            return requestParticipants.Select(participant => new ParticipantForJudgeResponse() {Case_type_group = participant.Case_type_group, Display_name = participant.Display_name, Representee = participant.Representee, Role = participant.User_role}).ToList();
        }
    }
}
