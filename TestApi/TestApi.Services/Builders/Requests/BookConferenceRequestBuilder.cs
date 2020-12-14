using System;
using System.Collections.Generic;
using AcceptanceTests.Common.Data.Helpers;
using TestApi.Common.Data;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Domain.Helpers;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.Services.Builders.Requests
{
    public class BookConferenceRequestBuilder
    {
        private readonly BookNewConferenceRequest _request;
        private readonly Random _randomNumber;
        private readonly List<User> _users;
        private readonly TestType _testType;
        private bool _isCACDCaseType;

        public BookConferenceRequestBuilder(List<User> users, TestType testType)
        {
            _request = new BookNewConferenceRequest();
            _randomNumber = new Random();
            _users = users;
            _testType = testType;
            _request.Case_type = HearingData.CASE_TYPE_NAME;
        }

        public BookConferenceRequestBuilder WithCACDCaseType()
        {
            _request.Case_type = HearingData.CACD_CASE_TYPE_NAME;
            _isCACDCaseType = true;
            return this;
        }

        public BookNewConferenceRequest BuildRequest()
        {
            _request.Audio_recording_required = HearingData.AUDIO_RECORDING_REQUIRED; 
            _request.Case_name = $"{AppShortName.FromApplication(Application.TestApi)} {GetCaseNamePrefix()} {GenerateRandom.Letters(_randomNumber)}";
            _request.Case_number = GenerateRandom.CaseNumber(_randomNumber);
            _request.Endpoints = new List<AddEndpointRequest>();
            _request.Hearing_ref_id = Guid.NewGuid();
            _request.Hearing_venue_name = HearingData.VENUE_NAME;
            _request.Participants = new BookConferenceParticipantsBuilder(_users, _isCACDCaseType).Build();
            _request.Scheduled_date_time = DateTime.UtcNow.AddMinutes(5);
            _request.Scheduled_duration = HearingData.SCHEDULED_DURATION;
            return _request;
        }

        private string GetCaseNamePrefix()
        {
            return _testType switch
            {
                TestType.Manual => HearingData.MANUAL_CASE_NAME_PREFIX,
                TestType.Performance => HearingData.PERFORMANCE_CASE_NAME_PREFIX,
                _ => HearingData.AUTOMATED_CASE_NAME_PREFIX
            };
        }
    }
}
