using System;
using System.Collections.Generic;
using System.Linq;
using AcceptanceTests.Common.Data.Helpers;
using TestApi.Common.Exceptions;
using TestApi.Contract.Requests;
using TestApi.Domain.Enums;
using TestApi.Domain.Helpers;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Builders
{
    public class BookHearingRequestBuilder
    {
        private const string CASE_TYPE_NAME = "Civil Money Claims";
        private const string HEARING_ROOM_NAME = "Room 1";
        private const string HEARING_TYPE_NAME = "Application to Set Judgment Aside";
        private const string OTHER_INFORMATION = "Other information";
        private const int SCHEDULED_DURATION = 60;
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

        private void AddCases()
        {
            var caseRequest = new CaseRequest
            {
                Name = GenerateRandomCaseName(),
                Number = GenerateRandom.CaseNumber(_randomNumber),
                AdditionalProperties = null,
                Is_lead_case = false
            };
            _request.Cases.Add(caseRequest);
        }

        private string GenerateRandomCaseName()
        {
            return
                $"{AppShortName.FromApplication(_createHearingRequest.Application)} Automated Test {GenerateRandom.Letters(_randomNumber)}";
        }

        private void SetCreatedBy()
        {
            if (_createHearingRequest.Users.All(x => x.UserType != UserType.CaseAdmin))
            {
                var usernames = _createHearingRequest.Users.Select(x => x.UserType.ToString()).ToList();
                throw new ParticipantsException(usernames);
            }

            _request.Created_by = _createHearingRequest.Users.First(x => x.UserType == UserType.CaseAdmin).Username;
        }

        public BookNewHearingRequest Build()
        {
            AddCases();
            SetCreatedBy();
            _request.AdditionalProperties = null;
            _request.Audio_recording_required = _createHearingRequest.AudioRecordingRequired;
            _request.Case_type_name = CASE_TYPE_NAME;
            _request.Hearing_room_name = HEARING_ROOM_NAME;
            _request.Hearing_type_name = HEARING_TYPE_NAME;
            _request.Hearing_venue_name = _createHearingRequest.Venue;
            _request.Other_information = OTHER_INFORMATION;
            _request.Questionnaire_not_required = _createHearingRequest.QuestionnaireNotRequired;
            _request.Participants = new BookHearingParticipantsBuilder(_createHearingRequest.Users).Build();
            _request.Scheduled_date_time = _createHearingRequest.ScheduledDateTime;
            _request.Scheduled_duration = SCHEDULED_DURATION;
            return _request;
        }
    }
}