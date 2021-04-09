using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.Data.Questions;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Contract.Dtos;
using TestApi.Contract.Requests;
using TestApi.Contract.Enums;
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

        protected CreateHearingRequest CreateHearingWithJustIndividualAndJudge()
        {
            var users = CreateUsersWithJustIndividualAndJudge();
            return new HearingBuilder(users).Build();
        }

        protected CreateHearingRequest CreateHearingWithJustRepAndJudge()
        {
            var users = CreateUsersWithJustRepAndJudge();
            return new HearingBuilder(users).Build();
        }

        protected CreateHearingRequest CreateCACDHearingRequest()
        {
            var judge = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddJudge()
                .BuildUserDto();

            var individual = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddIndividual()
                .BuildUserDto();

            var caseAdmin = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddCaseAdmin()
                .BuildUserDto();

            var winger = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddWinger()
                .BuildUserDto();

            var users = new List<UserDto>() { judge, individual, caseAdmin, winger };

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

        private List<UserDto> CreateDefaultUsers(TestType testType)
        {
            var judge = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddJudge()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUserDto();

            var individual = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddIndividual()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUserDto();

            var representative = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddRepresentative()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUserDto();

            var observer = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddObserver()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUserDto();

            var panelMember = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddPanelMember()
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUserDto();

            return new List<UserDto>() { judge, individual, representative, observer, panelMember };
        }

        private List<UserDto> CreateUsersWithJustIndividualAndJudge()
        {
            var judge = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddJudge()
                .ForApplication(Application.TestApi)
                .BuildUserDto();

            var individual = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddIndividual()
                .ForApplication(Application.TestApi)
                .BuildUserDto();

            return new List<UserDto>() { judge, individual };
        }

        private List<UserDto> CreateUsersWithJustRepAndJudge()
        {
            var judge = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddJudge()
                .ForApplication(Application.TestApi)
                .BuildUserDto();

            var representative = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddRepresentative()
                .ForApplication(Application.TestApi)
                .BuildUserDto();

            return new List<UserDto>() { judge, representative };
        }

        protected List<SuitabilityAnswersRequest> CreateAnswersRequest()
        {
            return new List<SuitabilityAnswersRequest>()
            {
                new SuitabilityAnswersRequest()
                {
                    Key = IndividualQuestionKeys.AboutYouQuestion,
                    ExtendedAnswer = null,
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
