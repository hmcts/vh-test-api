using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Contract.Requests;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.IntegrationTests.Configuration;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class HearingsTestsBase : ControllerTestsBase
    {
        protected readonly List<HearingDetailsResponse> HearingsToDelete = new List<HearingDetailsResponse>();

        protected CreateHearingRequest CreateHearingRequest()
        {
            var users = CreateDefaultUsers();
            return new HearingBuilder(users).BuildRequest();
        }

        protected async Task<HearingDetailsResponse> CreateHearing(CreateHearingRequest request)
        {
            var uri = ApiUriFactory.HearingEndpoints.CreateHearing;

            await SendPostRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.Created, true);

            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);
            response.Should().NotBeNull();

            HearingsToDelete.Add(response);

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

            var caseAdmin = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddCaseAdmin()
                .ForApplication(Application.TestApi)
                .BuildUser();

            return new List<User>() { judge, individual, representative, caseAdmin };
        }

        [TearDown]
        public async Task RemoveHearingData()
        {
            foreach (var hearing in HearingsToDelete)
            {
                await DeleteHearing(hearing.Id);
                VerifyResponse(HttpStatusCode.NoContent, true);
            }

            HearingsToDelete.Clear();
        }

        protected async Task DeleteHearing(Guid hearingId)
        {
            var uri = ApiUriFactory.HearingEndpoints.DeleteHearing(hearingId);
            await SendDeleteRequest(uri);
        }
    }
}
