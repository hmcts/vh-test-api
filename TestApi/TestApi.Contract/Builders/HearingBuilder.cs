using System;
using System.Collections.Generic;
using TestApi.Contract.Requests;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.Contract.Builders
{
    public class HearingBuilder
    {
        private const string DEFAULT_VENUE_NAME = "Birmingham Civil and Family Justice Centre";
        private readonly CreateHearingRequest _request;

        public HearingBuilder(List<User> users)
        {
            _request = new CreateHearingRequest
            {
                Application = Application.TestApi,
                AudioRecordingRequired = false,
                QuestionnaireNotRequired = true,
                ScheduledDateTime = DateTime.UtcNow,
                Users = users,
                Venue = DEFAULT_VENUE_NAME
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

        public CreateHearingRequest BuildRequest()
        {
            return _request;
        }
    }
}
