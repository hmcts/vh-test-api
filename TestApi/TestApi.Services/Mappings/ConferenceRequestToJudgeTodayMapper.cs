using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;

namespace TestApi.Services.Mappings
{
    public static class ConferenceRequestToJudgeTodayMapper
    {
        public static List<ConferenceForHostResponse> Map(ConferenceDetailsResponse response)
        {
            return new List<ConferenceForHostResponse>()
            {
                new ConferenceForHostResponse()
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

        private static List<ParticipantForHostResponse> AddParticipants(IEnumerable<ParticipantDetailsResponse> requestParticipants)
        {
            return requestParticipants.Select(participant => new ParticipantForHostResponse() {CaseTypeGroup = participant.CaseTypeGroup, DisplayName = participant.DisplayName, Representee = participant.Representee, Role = participant.UserRole}).ToList();
        }
    }
}
