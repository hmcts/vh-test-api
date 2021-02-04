using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using RestSharp;
using TestApi.AcceptanceTests.Helpers;
using TestApi.Common.Builders;
using TestApi.Contract.Responses;
using TestApi.Domain.Enums;
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
    }
}
