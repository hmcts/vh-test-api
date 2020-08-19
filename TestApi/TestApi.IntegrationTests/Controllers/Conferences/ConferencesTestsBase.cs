using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Builders;
using TestApi.Services.Builders.Requests;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class ConferencesTestsBase : ControllerTestsBase
    {
        protected readonly List<ConferenceDetailsResponse> ConferencesToDelete = new List<ConferenceDetailsResponse>();

        protected BookNewConferenceRequest CreateConferenceRequest()
        {
            var users = CreateDefaultUsers();
            return new BookConferenceRequestBuilder(users).BuildRequest();
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

        private List<User> CreateDefaultUsers()
        {
            var judge = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddJudge()
                .ForApplication(Application.TestApi)
                .BuildUser();

            var individual = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddIndividual()
                .ForApplication(Application.TestApi)
                .BuildUser();

            var representative = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddRepresentative()
                .ForApplication(Application.TestApi)
                .BuildUser();

            var observer = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddObserver()
                .ForApplication(Application.TestApi)
                .BuildUser();

            var panelMember = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddPanelMember()
                .ForApplication(Application.TestApi)
                .BuildUser();

            var caseAdmin = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddCaseAdmin()
                .ForApplication(Application.TestApi)
                .BuildUser();

            return new List<User>() { judge, individual, representative, observer, panelMember, caseAdmin };
        }

        [TearDown]
        public async Task RemoveHearingData()
        {
            foreach (var conference in ConferencesToDelete)
            {
                await DeleteConference(conference.Id);
                VerifyResponse(HttpStatusCode.NoContent, true);
            }

            ConferencesToDelete.Clear();
        }

        protected async Task DeleteConference(Guid conferenceId)
        {
            var uri = ApiUriFactory.ConferenceEndpoints.DeleteConference(conferenceId);
            await SendDeleteRequest(uri);
        }
    }
}
