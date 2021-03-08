using System;
using System.Collections.Generic;
using AcceptanceTests.Common.Data.Helpers;
using TestApi.Common.Data;
using TestApi.Contract.Dtos;
using TestApi.Contract.Enums;
using TestApi.Contract.Helpers;
using VideoApi.Contract.Requests;

namespace TestApi.Services.Builders.Requests
{
    public class BookConferenceRequestBuilder
    {
        private readonly BookNewConferenceRequest _request;
        private readonly Random _randomNumber;
        private readonly List<UserDto> _users;
        private readonly TestType _testType;
        private bool _isCACDCaseType;

        public BookConferenceRequestBuilder(List<UserDto> users, TestType testType)
        {
            _request = new BookNewConferenceRequest();
            _randomNumber = new Random();
            _users = users;
            _testType = testType;
            _request.CaseType = HearingData.CASE_TYPE_NAME;
        }

        public BookConferenceRequestBuilder WithCACDCaseType()
        {
            _request.CaseType = HearingData.CACD_CASE_TYPE_NAME;
            _isCACDCaseType = true;
            return this;
        }

        public BookNewConferenceRequest BuildRequest()
        {
            _request.AudioRecordingRequired = HearingData.AUDIO_RECORDING_REQUIRED; 
            _request.CaseName = $"{AppShortName.FromApplication(Application.TestApi)} {GetCaseNamePrefix()} {GenerateRandom.Letters(_randomNumber)}";
            _request.CaseNumber = GenerateRandom.CaseNumber(_randomNumber);
            _request.Endpoints = new List<AddEndpointRequest>();
            _request.HearingRefId = Guid.NewGuid();
            _request.HearingVenueName = HearingData.VENUE_NAME;
            _request.Participants = new BookConferenceParticipantsBuilder(_users, _isCACDCaseType).Build();
            _request.ScheduledDateTime = DateTime.UtcNow.AddMinutes(5);
            _request.ScheduledDuration = HearingData.SCHEDULED_DURATION;
            return _request;
        }

        private string GetCaseNamePrefix()
        {
            return _testType switch
            {
                TestType.Automated => HearingData.AUTOMATED_CASE_NAME_PREFIX,
                TestType.Demo => HearingData.DEMO_CASE_NAME_PREFIX,
                TestType.ITHC => HearingData.ITHC_CASE_NAME_PREFIX,
                TestType.Manual => HearingData.MANUAL_CASE_NAME_PREFIX,
                TestType.Performance => HearingData.PERFORMANCE_CASE_NAME_PREFIX,
                _ => HearingData.AUTOMATED_CASE_NAME_PREFIX
            };
        }
    }
}
