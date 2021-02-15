using System;
using System.Data;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
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
            var username = await AllocateUser();

            await PollForUserExistsInAad(username);

            var request = new ResetUserPasswordRequest()
            {
                Username = username
            };

            var uri = ApiUriFactory.UserEndpoints.ResetUserPassword();
            await SendPatchRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.OK, true);

            var response = RequestHelper.Deserialise<UpdateUserResponse>(Json);

            response.Should().NotBeNull();
            response.New_password.Should().NotBeNullOrEmpty();
        }

        private async Task<string> AllocateUser()
        {
            var request = new AllocateUserRequestBuilder().ForTestType(TestType.Manual).Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            return response.Username;
        }

        private async Task PollForUserExistsInAad(string username)
        {
            var getUserUri = ApiUriFactory.UserEndpoints.GetUserExistsInAd(username.ToUpperInvariant());
            const int RETRIES = 10;
            const int DELAY = 2;
            for (var i = 0; i < RETRIES; i++)
            {
                await SendGetRequest(getUserUri);
                if (Response.IsSuccessStatusCode)
                {
                    VerifyResponse(HttpStatusCode.OK, true);
                    var getUserResponse = RequestHelper.Deserialise<bool>(Json);
                    getUserResponse.Should().BeTrue();
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(DELAY));
            }

            throw new DataException($"Failed to find created user in AAD after {RETRIES * DELAY} seconds.");
        }

        [Test]
        public async Task Should_return_bad_request_for_automation_user()
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
            VerifyResponse(HttpStatusCode.BadRequest, false);
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
                Username = string.Empty
            };

            var uri = ApiUriFactory.UserEndpoints.ResetUserPassword();
            await SendPatchRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.BadRequest, false);
        }
    }
}
