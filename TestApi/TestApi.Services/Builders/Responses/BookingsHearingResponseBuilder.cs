using System.Linq;
using BookingsApi.Contract.Responses;

namespace TestApi.Services.Builders.Responses
{
    public class BookingsHearingResponseBuilder
    {
        private readonly BookingsHearingResponse _response;

        public BookingsHearingResponseBuilder(HearingDetailsResponse response)
        {
            _response = new BookingsHearingResponse()
            {
                AudioRecordingRequired = response.AudioRecordingRequired,
                CancelReason = response.CancelReason,
                CaseTypeName = response.CaseTypeName,
                ConfirmedBy = response.ConfirmedBy,
                ConfirmedDate = response.ConfirmedDate,
                CourtAddress = string.Empty,
                CourtRoom = string.Empty,
                CourtRoomAccount = string.Empty,
                CreatedBy = response.CreatedBy,
                CreatedDate = response.CreatedDate,
                GroupId = response.GroupId,
                HearingId = response.Id,
                HearingNumber = response.Cases.Single().Number,
                HearingName = response.Cases.Single().Name,
                HearingTypeName = response.HearingTypeName,
                JudgeName = response.Participants.Single(x => x.HearingRoleName.Equals("Judge")).DisplayName,
                LastEditBy = response.UpdatedBy,
                LastEditDate = response.UpdatedDate,
                QuestionnaireNotRequired = response.QuestionnaireNotRequired,
                ScheduledDateTime = response.ScheduledDateTime,
                ScheduledDuration = response.ScheduledDuration,
                Status = response.Status
            };
        }

        public BookingsHearingResponse Build()
        {
            return _response;
        }
    }
}
