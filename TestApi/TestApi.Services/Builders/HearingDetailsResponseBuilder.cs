using System;
using System.Collections.Generic;
using System.Linq;
using TestApi.Domain.Helpers;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Builders
{
    public class HearingDetailsResponseBuilder
    {
        private readonly BookNewHearingRequest _request;

        public HearingDetailsResponseBuilder(BookNewHearingRequest request)
        {
            _request = request;
        }


        public HearingDetailsResponse Build()
        {
            var cases = new List<CaseResponse>()
            {
                new CaseResponse()
                {
                    AdditionalProperties = null,
                    Is_lead_case = _request.Cases.First().Is_lead_case,
                    Name = _request.Cases.First().Name,
                    Number = _request.Cases.First().Number
                }
            };

            var participants = (from participant in _request.Participants
                let userType = GetUserType.FromUserLastName(participant.Last_name)
                let userRoleName = UserTypeName.FromUserType(userType)
                select new ParticipantResponse()
                {
                    AdditionalProperties = null,
                    Case_role_name = participant.Case_role_name,
                    Contact_email = participant.Contact_email,
                    Display_name = participant.Display_name,
                    First_name = participant.First_name,
                    Hearing_role_name = participant.Hearing_role_name,
                    Id = Guid.NewGuid(),
                    Last_name = participant.Last_name,
                    Middle_names = participant.Middle_names,
                    Organisation = participant.Organisation_name,
                    Reference = participant.Reference,
                    Representee = participant.Representee,
                    Telephone_number = participant.Telephone_number,
                    Title = participant.Title,
                    User_role_name = userRoleName,
                    Username = participant.Username
                }).ToList();

            return new HearingDetailsResponse()
            {
                AdditionalProperties = null,
                Audio_recording_required = _request.Audio_recording_required,
                Cancel_reason = null,
                Case_type_name = _request.Case_type_name,
                Cases = cases,
                Confirmed_by = null,
                Confirmed_date = null,
                Created_by = _request.Created_by,
                Created_date = DateTime.UtcNow,
                Hearing_room_name = _request.Hearing_room_name,
                Hearing_type_name = _request.Hearing_type_name,
                Hearing_venue_name = _request.Hearing_venue_name,
                Id = Guid.NewGuid(),
                Other_information = _request.Other_information,
                Participants = participants,
                Questionnaire_not_required = _request.Questionnaire_not_required,
                Scheduled_date_time = _request.Scheduled_date_time,
                Scheduled_duration = _request.Scheduled_duration,
                Status = BookingStatus.Booked,
                Updated_by = _request.Created_by,
                Updated_date = DateTime.UtcNow
            };
        }
    }
}
