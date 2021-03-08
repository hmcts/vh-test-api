using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;

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
                    CaseName = response.CaseName,
                    CaseNumber = response.CaseNumber,
                    CaseType = response.CaseType,
                    ClosedDateTime = response.ClosedDateTime,
                    HearingVenueName = response.HearingVenueName,
                    HearingRefId = response.HearingId,
                    Id = response.Id,
                    Participants = AddParticipants(response.Participants),
                    ScheduledDateTime = response.ScheduledDateTime,
                    ScheduledDuration = response.ScheduledDuration,
                    StartedDateTime = response.StartedDateTime,
                    Status = response.CurrentStatus
                }
            };
        }

        private static List<ParticipantSummaryResponse> AddParticipants(IEnumerable<ParticipantDetailsResponse> requestParticipants)
        {
            return requestParticipants.Select(participant => new ParticipantSummaryResponse()
                {
                    CaseGroup = participant.CaseTypeGroup,
                    ContactEmail = participant.ContactEmail,
                    ContactTelephone = participant.ContactTelephone,
                    DisplayName = participant.DisplayName,
                    FirstName = participant.FirstName,
                    Id = participant.Id,
                    LastName = participant.LastName,
                    LinkedParticipants = new List<LinkedParticipantResponse>(),
                    Representee = participant.Representee,
                    Status = participant.CurrentStatus,
                    UserRole = participant.UserRole,
                    Username = participant.Username
                })
                .ToList();
        }
    }
}
