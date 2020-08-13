using System;
using System.Collections.Generic;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.Contract.Requests
{
    /// <summary>Create a hearing request</summary>
    public class CreateHearingRequest
    {
        /// <summary>Records the hearing audio</summary>
        public bool AudioRecordingRequired { get; set; }

        /// <summary>Application hearing is required for</summary>
        public Application Application { get; set; }

        /// <summary>Participants need to answer questionnaire before video web</summary>
        public bool QuestionnaireNotRequired { get; set; }

        /// <summary>Hearing scheduled date and time</summary>
        public DateTime ScheduledDateTime { get; set; }

        /// <summary>List of users to include as participants</summary>
        public List<User> Users { get; set; }

        /// <summary>Hearing venue name</summary>
        public string Venue { get; set; }
    }
}