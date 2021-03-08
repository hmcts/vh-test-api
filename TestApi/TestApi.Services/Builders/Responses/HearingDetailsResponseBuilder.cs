using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Enums;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using TestApi.Domain.Helpers;

namespace TestApi.Services.Builders.Responses
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
            var cases = new List<CaseResponse>
            {
                new CaseResponse
                {
                    IsLeadCase = _request.Cases.First().IsLeadCase,
                    Name = _request.Cases.First().Name,
                    Number = _request.Cases.First().Number
                }
            };

            var participants = (from participant in _request.Participants
                let userType = GetUserType.FromUserLastName(participant.LastName)
                let userRoleName = UserTypeName.FromUserType(userType)
                select new ParticipantResponse
                {
                    CaseRoleName = participant.CaseRoleName,
                    ContactEmail = participant.ContactEmail,
                    DisplayName = participant.DisplayName,
                    FirstName = participant.FirstName,
                    HearingRoleName = participant.HearingRoleName,
                    Id = Guid.NewGuid(),
                    LastName = participant.LastName,
                    MiddleNames = participant.MiddleNames,
                    Organisation = participant.OrganisationName,
                    Representee = participant.Representee,
                    TelephoneNumber = participant.TelephoneNumber,
                    Title = participant.Title,
                    UserRoleName = userRoleName,
                    Username = participant.Username
                }).ToList();

            return new HearingDetailsResponse
            {
                AudioRecordingRequired = _request.AudioRecordingRequired,
                CancelReason = null,
                CaseTypeName = _request.CaseTypeName,
                Cases = cases,
                ConfirmedBy = null,
                ConfirmedDate = null,
                CreatedBy = _request.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                HearingRoomName = _request.HearingRoomName,
                HearingTypeName = _request.HearingTypeName,
                HearingVenueName = _request.HearingVenueName,
                Id = Guid.NewGuid(),
                OtherInformation = _request.OtherInformation,
                Participants = participants,
                QuestionnaireNotRequired = _request.QuestionnaireNotRequired,
                ScheduledDateTime = _request.ScheduledDateTime,
                ScheduledDuration = _request.ScheduledDuration,
                Status = BookingStatus.Booked,
                UpdatedBy = _request.CreatedBy,
                UpdatedDate = DateTime.UtcNow
            };
        }
    }
}