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

        public BookConferenceRequestBuilder(List<User> users)
        {
            _request = new BookNewConferenceRequest();
            _randomNumber = new Random();
            _users = users;
        }

        public BookNewConferenceRequest BuildRequest()
        {
            _request.Audio_recording_required = DefaultData.AUDIO_RECORDING_REQUIRED; 
            _request.Case_name = $"{AppShortName.FromApplication(Application.TestApi)} {DefaultData.CASE_NAME_PREFIX} {GenerateRandom.Letters(_randomNumber)}";
            _request.Case_number = GenerateRandom.CaseNumber(_randomNumber);
            _request.Case_type = DefaultData.CASE_TYPE_NAME;
            _request.Hearing_ref_id = Guid.NewGuid();
            _request.Hearing_venue_name = DefaultData.VENUE_NAME;
            _request.Participants = new BookConferenceParticipantsBuilder(_users).Build();
            _request.Scheduled_date_time = DateTime.UtcNow.AddMinutes(5);
            _request.Scheduled_duration = DefaultData.SCHEDULED_DURATION;
            return _request;
        }
    }
}
