using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;

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
                    CaseName = response.CaseName,
                    CaseNumber = response.CaseNumber,
                    CaseType = response.CaseType,
                    Id = Guid.NewGuid(),
                    Participants = AddParticipants(response.Participants),
                    ScheduledDateTime = response.ScheduledDateTime,
                    ScheduledDuration = response.ScheduledDuration,
                    Status = ConferenceState.NotStarted
                }
            };
        }

        private static List<ParticipantForJudgeResponse> AddParticipants(IEnumerable<ParticipantDetailsResponse> requestParticipants)
        {
            return requestParticipants.Select(participant => new ParticipantForJudgeResponse() {CaseTypeGroup = participant.CaseTypeGroup, DisplayName = participant.DisplayName, Representee = participant.Representee, Role = participant.UserRole}).ToList();
        }
    }
}
