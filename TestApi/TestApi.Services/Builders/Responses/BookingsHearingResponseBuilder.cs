using System.Linq;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Builders.Responses
{
    public class BookingsHearingResponseBuilder
    {
        private readonly BookingsHearingResponse _response;

        public BookingsHearingResponseBuilder(HearingDetailsResponse response)
        {
            _response = new BookingsHearingResponse()
            {
                Audio_recording_required = response.Audio_recording_required,
                Cancel_reason = response.Cancel_reason,
                Case_type_name = response.Case_type_name,
                Confirmed_by = response.Confirmed_by,
                Confirmed_date = response.Confirmed_date,
                Court_address = string.Empty,
                Court_room = string.Empty,
                Court_room_account = string.Empty,
                Created_by = response.Created_by,
                Created_date = response.Created_date,
                Group_id = response.Group_id,
                Hearing_date = response.Scheduled_date_time,
                Hearing_id = response.Id,
                Hearing_number = response.Cases.Single().Number,
                Hearing_name = response.Cases.Single().Name,
                Hearing_type_name = response.Hearing_type_name,
                Judge_name = response.Participants.Single(x => x.Hearing_role_name.Equals("Judge")).Display_name,
                Last_edit_by = response.Updated_by,
                Last_edit_date = response.Updated_date,
                Questionnaire_not_required = response.Questionnaire_not_required,
                Scheduled_date_time = response.Scheduled_date_time,
                Scheduled_duration = response.Scheduled_duration,
                Status = response.Status
            };
        }

        public BookingsHearingResponse Build()
        {
            return _response;
        }
    }
}
