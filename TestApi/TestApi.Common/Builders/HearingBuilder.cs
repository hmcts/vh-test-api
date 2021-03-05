using System;
using System.Collections.Generic;
using TestApi.Common.Data;
using TestApi.Contract.Dtos;
using TestApi.Contract.Requests;
using TestApi.Contract.Enums;

namespace TestApi.Common.Builders
{
    public class HearingBuilder
    {
        private readonly CreateHearingRequest _request;

        public HearingBuilder(List<UserDto> users)
        {
            _request = new CreateHearingRequest
            {
                Application = Application.TestApi,
                AudioRecordingRequired = HearingData.AUDIO_RECORDING_REQUIRED,
                CaseType = HearingData.CASE_TYPE_NAME,
                Endpoints = HearingData.NUMBER_OF_ENDPOINTS,
                QuestionnaireNotRequired = HearingData.QUESTIONNAIRE_NOT_REQUIRED,
                ScheduledDateTime = DateTime.UtcNow,
                TestType = TestType.Automated,
                Users = users,
                Venue = HearingData.VENUE_NAME
            };
        }

        public HearingBuilder AudioRecordingRequired()
        {
            _request.AudioRecordingRequired = true;
            return this;
        }

        public HearingBuilder CACDHearing()
        {
            _request.CaseType = HearingData.CACD_CASE_TYPE_NAME;
            return this;
        }

        public HearingBuilder ForApplication(Application application)
        {
            _request.Application = application;
            return this;
        }

        public HearingBuilder HearingVenue(string venue)
        {
            _request.Venue = venue;
            return this;
        }

        public HearingBuilder QuestionnairesRequired()
        {
            _request.QuestionnaireNotRequired = false;
            return this;
        }

        public HearingBuilder ScheduledDateTime(DateTime dateTime)
        {
            _request.ScheduledDateTime = dateTime;
            return this;
        }

        public HearingBuilder TypeOfTest(TestType testType)
        {
            _request.TestType = testType;
            return this;
        }

        public HearingBuilder WithoutACaseType()
        {
            _request.CaseType = string.Empty;
            return this;
        }

        public CreateHearingRequest Build()
        {
            return _request;
        }
    }
}