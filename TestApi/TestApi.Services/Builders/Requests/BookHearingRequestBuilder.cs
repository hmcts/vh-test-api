using System;
using System.Collections.Generic;
using System.Linq;
using AcceptanceTests.Common.Data.Helpers;
using Castle.Core.Internal;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Domain.Enums;
using TestApi.Domain.Helpers;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Builders.Requests
{
    public class BookHearingRequestBuilder
    {
        private readonly CreateHearingRequest _createHearingRequest;
        private readonly Random _randomNumber;
        private readonly BookNewHearingRequest _request;

        public BookHearingRequestBuilder(CreateHearingRequest createHearingRequest)
        {
            _request = new BookNewHearingRequest
            {
                Case_type_name = createHearingRequest.CaseType,
                Cases = new List<CaseRequest>(),
                Participants = new List<ParticipantRequest>()
            };
            _createHearingRequest = createHearingRequest;
            _randomNumber = new Random();
        }

        private void AddCases()
        {
            var caseRequest = new CaseRequest
            {
                Name = GenerateRandomCaseName(),
                Number = GenerateRandom.CaseNumber(_randomNumber),
                AdditionalProperties = null,
                Is_lead_case = HearingData.IS_LEAD_CASE
            };
            _request.Cases.Add(caseRequest);
        }

        private string GenerateRandomCaseName()
        {
            return
                $"{GetCustomCaseNamePrefix()}{AppShortName.FromApplication(_createHearingRequest.Application)} {GetCaseNamePrefix()} {GenerateRandom.Letters(_randomNumber)}";
        }

        private string GetCaseNamePrefix()
        {
            return _createHearingRequest.TestType switch
            {
                TestType.ITHC => HearingData.ITHC_CASE_NAME_PREFIX,
                TestType.Manual => HearingData.MANUAL_CASE_NAME_PREFIX,
                TestType.Performance => HearingData.PERFORMANCE_CASE_NAME_PREFIX,
                TestType.Demo => HearingData.DEMO_CASE_NAME_PREFIX,
                _ => HearingData.AUTOMATED_CASE_NAME_PREFIX
            };
        }

        private string GetCustomCaseNamePrefix()
        {
            return string.IsNullOrEmpty(_createHearingRequest.CustomCaseNamePrefix) ? string.Empty : $"{_createHearingRequest.CustomCaseNamePrefix} ";
        }

        private void AddEndpoints()
        {
            if (_createHearingRequest.Endpoints <= 0) return;
            _request.Endpoints = new List<EndpointRequest>();

            for (var i = 1; i <= _createHearingRequest.Endpoints; i++)
            {
                _request.Endpoints.Add(new EndpointRequest()
                {
                    Display_name = $"{HearingData.ENDPOINT_PREFIX}{i}"
                });
            }
        }

        private void SetCreatedBy()
        {
            if (!_createHearingRequest.CreatedBy.IsNullOrEmpty())
            {
                _request.Created_by = _createHearingRequest.CreatedBy;
                return;
            }

            var caseAdminsCount = _createHearingRequest.Users.Count(x => x.UserType == UserType.CaseAdmin);
            var videoHearingsOfficerCount = _createHearingRequest.Users.Count(x => x.UserType == UserType.VideoHearingsOfficer);
            
            if (caseAdminsCount + videoHearingsOfficerCount == 0)
            {
                _request.Created_by = UserData.DEFAULT_CREATED_BY_USER;
            }

            if (caseAdminsCount > 0)
            {
                _request.Created_by = _createHearingRequest.Users.First(x => x.UserType == UserType.CaseAdmin).Username;
            }

            if (videoHearingsOfficerCount > 0)
            {
                _request.Created_by = _createHearingRequest.Users.First(x => x.UserType == UserType.VideoHearingsOfficer).Username;
            }
        }

        private void SetCaseTypeAndHearingTypeNames()
        {
            if (_request.Case_type_name.IsNullOrEmpty())
            {
                _request.Case_type_name = HearingData.CASE_TYPE_NAME;
                _request.Hearing_type_name = HearingData.HEARING_TYPE_NAME;
            }
            else
            {
                _request.Hearing_type_name = IsCACDCaseType() ? HearingData.CACD_HEARING_TYPE_NAME : HearingData.HEARING_TYPE_NAME;
            }
        }

        public BookNewHearingRequest Build()
        {
            AddCases();
            SetCreatedBy();
            SetCaseTypeAndHearingTypeNames();
            AddEndpoints();
            _request.AdditionalProperties = null;
            _request.Audio_recording_required = _createHearingRequest.AudioRecordingRequired;
            _request.Hearing_room_name = HearingData.HEARING_ROOM_NAME;
            _request.Hearing_venue_name = _createHearingRequest.Venue;
            _request.Other_information = HearingData.OTHER_INFORMATION;
            _request.Questionnaire_not_required = _createHearingRequest.QuestionnaireNotRequired;
            _request.Participants = new BookHearingParticipantsBuilder(_createHearingRequest.Users, IsCACDCaseType()).Build();
            _request.Scheduled_date_time = _createHearingRequest.ScheduledDateTime;
            _request.Scheduled_duration = HearingData.SCHEDULED_DURATION;
            return _request;
        }

        private bool IsCACDCaseType()
        {
            return _request.Case_type_name.Equals(HearingData.CACD_CASE_TYPE_NAME);
        }
    }
}