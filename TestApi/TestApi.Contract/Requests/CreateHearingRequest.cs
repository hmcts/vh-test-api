﻿using System;
using System.Collections.Generic;
using TestApi.Contract.Dtos;
using TestApi.Contract.Enums;

namespace TestApi.Contract.Requests
{
    /// <summary>Create a hearing request</summary>
    public class CreateHearingRequest
    {
        /// <summary>Records the hearing audio</summary>
        public bool AudioRecordingRequired { get; set; }

        /// <summary>Application hearing is required for</summary>
        public Application Application { get; set; }

        /// <summary>Case Type</summary>
        public string CaseType { get; set; }

        /// <summary>The user that created the hearing</summary>
        public string CreatedBy { get; set; }

        /// <summary>An optional parameter to add some text before the case name to help identify a case</summary>
        public string CustomCaseNamePrefix { get; set; }

        /// <summary>Endpoints</summary>
        public int Endpoints { get; set; }

        /// <summary>Participants need to answer questionnaire before video web</summary>
        public bool QuestionnaireNotRequired { get; set; }

        /// <summary>Hearing scheduled date and time</summary>
        public DateTime ScheduledDateTime { get; set; }

        /// <summary>Automation, Manual or Performance</summary>
        public TestType TestType { get; set; }

        /// <summary>List of users to include as participants</summary>
        public List<UserDto> Users { get; set; }

        /// <summary>Hearing venue name</summary>
        public string Venue { get; set; }
    }
}