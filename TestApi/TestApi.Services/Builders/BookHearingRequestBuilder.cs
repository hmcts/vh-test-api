using System;
using System.Collections.Generic;
using System.Linq;
using AcceptanceTests.Common.Data.Helpers;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Domain.Helpers;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Builders
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
                Cases = new List<CaseRequest>(),
                Participants = new List<ParticipantRequest>()
            };
            _createHearingRequest = createHearingRequest;
            _randomNumber = new Random();
        }

        public BookHearingRequestBuilder(string usernameStem)
        {
            _request = new BookNewHearingRequest
            {
                Cases = new List<CaseRequest>(),
                Participants = new List<ParticipantRequest>()
            };

            var users = new List<User>()
            {
                new UserBuilder(usernameStem, 1).AddJudge().ForApplication(Application.QueueSubscriber).BuildUser(),
                new UserBuilder(usernameStem, 1).AddIndividual().ForApplication(Application.QueueSubscriber).BuildUser(),
                new UserBuilder(usernameStem, 1).AddRepresentative().ForApplication(Application.QueueSubscriber).BuildUser(),
                new UserBuilder(usernameStem, 1).AddCaseAdmin().ForApplication(Application.QueueSubscriber).BuildUser(),
            };

            _createHearingRequest = new CreateHearingRequest()
            {
                Application = Application.TestApi,
                AudioRecordingRequired = DefaultData.AUDIO_RECORDING_REQUIRED,
                QuestionnaireNotRequired = DefaultData.QUESTIONNAIRE_NOT_REQUIRED,
                ScheduledDateTime = DateTime.UtcNow.AddMinutes(35),
                Users = users,
                Venue = DefaultData.VENUE_NAME
            };
            
            _randomNumber = new Random();
        }

        private void AddCases()
        {
            var caseRequest = new CaseRequest
            {
                Name = GenerateRandomCaseName(),
                Number = GenerateRandom.CaseNumber(_randomNumber),
                AdditionalProperties = null,
                Is_lead_case = DefaultData.IS_LEAD_CASE
            };
            _request.Cases.Add(caseRequest);
        }

        private string GenerateRandomCaseName()
        {
            return
                $"{AppShortName.FromApplication(_createHearingRequest.Application)} {DefaultData.CASE_NAME_PREFIX} {GenerateRandom.Letters(_randomNumber)}";
        }

        private void SetCreatedBy()
        {
            _request.Created_by = _createHearingRequest.Users.First(x => x.UserType == UserType.CaseAdmin).Username;
        }

        public BookNewHearingRequest Build()
        {
            AddCases();
            SetCreatedBy();
            _request.AdditionalProperties = null;
            _request.Audio_recording_required = _createHearingRequest.AudioRecordingRequired;
            _request.Case_type_name = DefaultData.CASE_TYPE_NAME;
            _request.Hearing_room_name = DefaultData.HEARING_ROOM_NAME;
            _request.Hearing_type_name = DefaultData.HEARING_TYPE_NAME;
            _request.Hearing_venue_name = _createHearingRequest.Venue;
            _request.Other_information = DefaultData.OTHER_INFORMATION;
            _request.Questionnaire_not_required = _createHearingRequest.QuestionnaireNotRequired;
            _request.Participants = new BookHearingParticipantsBuilder(_createHearingRequest.Users).Build();
            _request.Scheduled_date_time = _createHearingRequest.ScheduledDateTime;
            _request.Scheduled_duration = DefaultData.SCHEDULED_DURATION;
            return _request;
        }
    }
}