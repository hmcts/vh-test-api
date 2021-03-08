using System;
using System.Collections.Generic;
using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using RestSharp;
using TestApi.AcceptanceTests.Helpers;
using TestApi.Common.Builders;
using TestApi.Contract.Dtos;
using TestApi.Contract.Responses;
using TestApi.Contract.Enums;
using TestApi.Tests.Common.Configuration;
using TestContext = TestApi.AcceptanceTests.Helpers.TestContext;

namespace TestApi.AcceptanceTests.Controllers
{
    public class TestsBase
    {
        protected TestContext Context;
        protected RequestHandler RequestHandler;

        [SetUp]
        public void SetUpConfig()
        {
            Context = new TestContext{Config = new Config()};
            Context = new Setup().RegisterSecrets(Context);
            RequestHandler = new RequestHandler(Context.Config.Services.TestApiUrl, Context.Token);
        }

        [TearDown]
        public void TearDown()
        {
            if (Context?.TestData?.Hearing != null)
            {
                DeleteHearing(Context.TestData.Hearing.Id);
            }
        }

        protected IRestResponse SendRequest(IRestRequest request)
        {
            return RequestHandler.Client().Execute(request);
        }

        protected UserDetailsResponse AllocateUser(UserType userType)
        {
            var body = new AllocateUserRequestBuilder().WithUserType(userType).Build();
            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            var request = RequestHandler.Patch(uri, body);
            var response = SendRequest(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccessful.Should().BeTrue();
            return RequestHelper.Deserialise<UserDetailsResponse>(response.Content);
        }

        protected HearingDetailsResponse CreateHearing()
        {
            var users = CreateUsers();
            var body = new HearingBuilder(users).Build();
            var uri = ApiUriFactory.HearingEndpoints.CreateHearing;
            var request = RequestHandler.Post(uri, body);
            var response = SendRequest(request);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.IsSuccessful.Should().BeTrue();
            Context.TestData.Hearing = RequestHelper.Deserialise<HearingDetailsResponse>(response.Content);
            return Context.TestData.Hearing;
        }

        protected IRestResponse DeleteHearing(Guid hearingId)
        {
            var uri = ApiUriFactory.HearingEndpoints.DeleteHearing(hearingId);
            var request = RequestHandler.Delete(uri);
            var response = SendRequest(request);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            response.IsSuccessful.Should().BeTrue();
            Context.TestData.Hearing = null;
            Context.TestData.Conference = null;
            return response;
        }

        private List<UserDto> CreateUsers()
        {
            var judge = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddJudge()
                .ForApplication(Application.TestApi)
                .BuildUserDto();

            var individual = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddIndividual()
                .ForApplication(Application.TestApi)
                .BuildUserDto();

            return new List<UserDto>{ judge, individual };
        }
    }
}
