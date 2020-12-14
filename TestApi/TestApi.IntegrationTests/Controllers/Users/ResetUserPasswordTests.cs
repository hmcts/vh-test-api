using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Users
{
    public class ResetUserPasswordTests : ControllerTestsBase
    {
        [Test]
        public async Task Should_reset_users_password()
        {
            var individualUser = await Context.Data.SeedUser(UserType.Individual);
            await Context.Data.SeedAllocation(individualUser.Id);
            await Context.Data.AllocateUser(individualUser.Id);

            var request = new ResetUserPasswordRequest()
            {
                Username = individualUser.Username
            };

            var uri = ApiUriFactory.UserEndpoints.ResetUserPassword();
            await SendPatchRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.OK, true);

            var response = RequestHelper.Deserialise<UpdateUserResponse>(Json);

            response.Should().NotBeNull();
            response.New_password.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_username()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            var request = new ResetUserPasswordRequest()
            {
                Username = USERNAME
            };

            var uri = ApiUriFactory.UserEndpoints.ResetUserPassword();
            await SendPatchRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.NotFound, false);
        }

        [Test]
        public async Task Should_return_bad_request_for_null_username()
        {
            var request = new ResetUserPasswordRequest()
            {
                Username = null
            };

            var uri = ApiUriFactory.UserEndpoints.ResetUserPassword();
            await SendPatchRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.BadRequest, false);
        }
    }
}
