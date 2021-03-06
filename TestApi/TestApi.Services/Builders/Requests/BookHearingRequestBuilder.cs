using System;
using System.Collections.Generic;
using System.Linq;
using AcceptanceTests.Common.Data.Helpers;
using BookingsApi.Contract.Enums;
using BookingsApi.Contract.Requests;
using Castle.Core.Internal;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Contract.Enums;
using TestApi.Contract.Helpers;

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
                CaseTypeName = createHearingRequest.CaseType,
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
                IsLeadCase = HearingData.IS_LEAD_CASE
            };
            _request.Cases.Add(caseRequest);
        }

        private string GenerateRandomCaseName()
        {
            return
                $"{GetCustomCaseNamePrefix()}{GetAppShortName()}{GetCaseNamePrefix()} {GenerateRandom.Letters(_randomNumber)}";
        }

        private string GetCustomCaseNamePrefix()
        {
            return string.IsNullOrEmpty(_createHearingRequest.CustomCaseNamePrefix) ? string.Empty : $"{_createHearingRequest.CustomCaseNamePrefix} ";
        }

        private string GetAppShortName()
        {
            return _createHearingRequest.TestType == TestType.Automated ? $"{AppShortName.FromApplication(_createHearingRequest.Application)} " : string.Empty;
        }

        private string GetCaseNamePrefix()
        {
            return _createHearingRequest.TestType switch
            {
                TestType.Automated => HearingData.AUTOMATED_CASE_NAME_PREFIX,
                TestType.Demo => HearingData.DEMO_CASE_NAME_PREFIX,
                TestType.ITHC => HearingData.ITHC_CASE_NAME_PREFIX,
                TestType.Manual => HearingData.MANUAL_CASE_NAME_PREFIX,
                TestType.Performance => HearingData.PERFORMANCE_CASE_NAME_PREFIX,
                _ => HearingData.AUTOMATED_CASE_NAME_PREFIX
            };
        }

        private void AddEndpoints()
        {
            if (_createHearingRequest.Endpoints <= 0) return;
            _request.Endpoints = new List<EndpointRequest>();

            for (var i = 1; i <= _createHearingRequest.Endpoints; i++)
            {
                _request.Endpoints.Add(new EndpointRequest()
                {
                    DisplayName = $"{HearingData.ENDPOINT_PREFIX}{i}"
                });
            }
        }

        private void SetCreatedBy()
        {
            if (!_createHearingRequest.CreatedBy.IsNullOrEmpty())
            {
                _request.CreatedBy = _createHearingRequest.CreatedBy;
                return;
            }

            var caseAdminsCount = _createHearingRequest.Users.Count(x => x.UserType == UserType.CaseAdmin);
            var videoHearingsOfficerCount = _createHearingRequest.Users.Count(x => x.UserType == UserType.VideoHearingsOfficer);
            
            if (caseAdminsCount + videoHearingsOfficerCount == 0)
            {
                _request.CreatedBy = UserData.DEFAULT_CREATED_BY_USER;
            }

            if (caseAdminsCount > 0)
            {
                _request.CreatedBy = _createHearingRequest.Users.First(x => x.UserType == UserType.CaseAdmin).Username;
            }

            if (videoHearingsOfficerCount > 0)
            {
                _request.CreatedBy = _createHearingRequest.Users.First(x => x.UserType == UserType.VideoHearingsOfficer).Username;
            }
        }

        private void SetCaseTypeAndHearingTypeNames()
        {
            if (_request.CaseTypeName.IsNullOrEmpty())
            {
                _request.CaseTypeName = HearingData.CASE_TYPE_NAME;
                _request.HearingTypeName = HearingData.HEARING_TYPE_NAME;
            }
            else
            {
                _request.HearingTypeName = IsCACDCaseType() ? HearingData.CACD_HEARING_TYPE_NAME : HearingData.HEARING_TYPE_NAME;
            }
        }

        public BookNewHearingRequest Build()
        {
            AddCases();
            SetCreatedBy();
            SetCaseTypeAndHearingTypeNames();
            AddEndpoints();
            _request.AudioRecordingRequired = _createHearingRequest.AudioRecordingRequired;
            _request.HearingRoomName = HearingData.HEARING_ROOM_NAME;
            _request.HearingVenueName = _createHearingRequest.Venue;
            _request.OtherInformation = HearingData.OTHER_INFORMATION;
            _request.QuestionnaireNotRequired = _createHearingRequest.QuestionnaireNotRequired;
            _request.Participants = new BookHearingParticipantsBuilder(_createHearingRequest.Users, IsCACDCaseType()).Build();
            _request.ScheduledDateTime = _createHearingRequest.ScheduledDateTime;
            _request.ScheduledDuration = HearingData.SCHEDULED_DURATION;
            AddLinkedParticipants();
            return _request;
        }

        private void AddLinkedParticipants()
        {
            _request.LinkedParticipants = new List<LinkedParticipantRequest>();

            var interpreters = _createHearingRequest.Users.Where(x => x.UserType == UserType.Interpreter).ToList();
            var individuals = _createHearingRequest.Users.Where(x => x.UserType == UserType.Individual).ToList();

            for (var i = 0; i < interpreters.Count; i++)
            {
                _request.LinkedParticipants.Add(new LinkedParticipantRequest()
                {
                    ParticipantContactEmail = interpreters[i].ContactEmail,
                    LinkedParticipantContactEmail = individuals[i].ContactEmail,
                    Type = LinkedParticipantType.Interpreter
                });
            }
        }

        private bool IsCACDCaseType()
        {
            return _request.CaseTypeName.Equals(HearingData.CACD_CASE_TYPE_NAME);
        }
    }
}