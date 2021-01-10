using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.Data.Questions;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Contract.Requests;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class HearingsTestsBase : ControllerTestsBase
    {
        protected readonly List<HearingDetailsResponse> HearingsToDelete = new List<HearingDetailsResponse>();

        protected CreateHearingRequest CreateHearingRequest(TestType testType = TestType.Automated)
        {
            var users = CreateDefaultUsers(testType);
            return new HearingBuilder(users).TypeOfTest(testType).Build();
        }

        protected CreateHearingRequest CreateHearingWithJustIndividual()
        {
            var users = CreateUsersWithJustIndividual();
            return new HearingBuilder(users).Build();
        }

        protected CreateHearingRequest CreateHearingWithJustRep()
        {
            var users = CreateUsersWithJustRep();
            return new HearingBuilder(users).Build();
        }

        protected CreateHearingRequest CreateCACDHearingRequest()
        {
            var judge = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddJudge()
                .BuildUser();

            var individual = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddIndividual()
                .BuildUser();

            var caseAdmin = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddCaseAdmin()
                .BuildUser();

            var winger = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddWinger()
                .BuildUser();

            var users = new List<User>() { judge, individual, caseAdmin, winger };

            return new HearingBuilder(users).CACDHearing().Build();
        }

        protected CreateHearingRequest CreateHearingRequestWithQuestionnaireEnabled()
        {
            var users = CreateDefaultUsers(TestType.Automated);
            return new HearingBuilder(users).QuestionnairesRequired().Build();
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

        private List<User> CreateUsersWithJustIndividual()
        {
            var judge = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddJudge()
                .ForApplication(Application.TestApi)
                .BuildUser();

            var individual = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddIndividual()
                .ForApplication(Application.TestApi)
                .BuildUser();

            return new List<User>() { judge, individual };
        }

        private List<User> CreateUsersWithJustRep()
        {
            var judge = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddJudge()
                .ForApplication(Application.TestApi)
                .BuildUser();

            var representative = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddRepresentative()
                .ForApplication(Application.TestApi)
                .BuildUser();

            return new List<User>() { judge, representative };
        }

        protected List<SuitabilityAnswersRequest> CreateAnswersRequest()
        {
            return new List<SuitabilityAnswersRequest>()
            {
                new SuitabilityAnswersRequest()
                {
                    Key = IndividualQuestionKeys.AboutYouQuestion,
                    Extended_answer = null,
                    Answer = "Yes"
                }
            };
        }

        protected async Task CreateSuitabilityAnswers(List<SuitabilityAnswersRequest> request, Guid hearingId, Guid participantId)
        {
            var uri = ApiUriFactory.HearingEndpoints.UpdateSuitabilityAnswers(hearingId, participantId);
            await SendPutRequest(uri, RequestHelper.Serialise(request));
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
