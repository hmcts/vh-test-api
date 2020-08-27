﻿using System;
using System.Collections.Generic;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.Common.Builders
{
    public class HearingBuilder
    {
        private readonly CreateHearingRequest _request;

        public HearingBuilder(List<User> users)
        {
            _request = new CreateHearingRequest
            {
                Application = Application.TestApi,
                AudioRecordingRequired = HearingData.AUDIO_RECORDING_REQUIRED,
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

        public CreateHearingRequest Build()
        {
            return _request;
        }
    }
}