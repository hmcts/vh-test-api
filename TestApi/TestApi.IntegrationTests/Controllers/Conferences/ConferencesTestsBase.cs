using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Builders.Requests;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class ConferencesTestsBase : ControllerTestsBase
    {
        protected readonly List<ConferenceDetailsResponse> ConferencesToDelete = new List<ConferenceDetailsResponse>();

        protected BookNewConferenceRequest CreateConferenceRequest(TestType testType = TestType.Automated)
        {
            var users = CreateDefaultUsers(testType);
            return new BookConferenceRequestBuilder(users, testType).BuildRequest();
        }

        protected async Task<ConferenceDetailsResponse> CreateConference(BookNewConferenceRequest request)
        {
            var uri = ApiUriFactory.ConferenceEndpoints.CreateConference;

            await SendPostRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.Created, true);

            var response = RequestHelper.Deserialise<ConferenceDetailsResponse>(Json);
            response.Should().NotBeNull();

            ConferencesToDelete.Add(response);

            return response;
        }

        private List<User> CreateDefaultUsers(TestType testType)
        {
            var judge = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddJudge()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUser();

            var individual = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddIndividual()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUser();

            var representative = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddRepresentative()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUser();

            var observer = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddObserver()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUser();

            var panelMember = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddPanelMember()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUser();

            var caseAdmin = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddCaseAdmin()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUser();

            return new List<User>() { judge, individual, representative, observer, panelMember, caseAdmin };
        }

        protected ConferenceEventRequest CreateVideoEventRequest(ConferenceDetailsResponse conference)
        {
            const EventType EVENT_TYPE = EventType.MediaPermissionDenied;
            const int EVENT_TYPE_ID = (int)EVENT_TYPE;
            var participant = conference.Participants.First(x => x.User_role == UserRole.Individual);

            return new ConferenceEventRequest()
            {
                Conference_id = conference.Id.ToString(),
                Event_id = EVENT_TYPE_ID.ToString(),
                Event_type = EVENT_TYPE,
                Participant_id = participant.Id.ToString(),
                Reason = HearingData.VIDEO_EVENT_REASON,
                Time_stamp_utc = DateTime.UtcNow,
                Transfer_from = null,
                Transfer_to = null
            };
        }

        protected async Task CreateVideoEvent(ConferenceEventRequest request)
        {
            var uri = ApiUriFactory.ConferenceEndpoints.CreateVideoEvent;
            await SendPostRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.NoContent, true);
        }

        [TearDown]
        public async Task RemoveConferenceData()
        {
            foreach (var conference in ConferencesToDelete)
            {
                await DeleteConference(conference.Hearing_id, conference.Id);
                VerifyResponse(HttpStatusCode.NoContent, true);
            }

            ConferencesToDelete.Clear();
        }

        protected async Task DeleteConference(Guid hearingRefId, Guid conferenceId)
        {
            var uri = ApiUriFactory.ConferenceEndpoints.DeleteConference(hearingRefId, conferenceId);
            await SendDeleteRequest(uri);
        }
    }
}
