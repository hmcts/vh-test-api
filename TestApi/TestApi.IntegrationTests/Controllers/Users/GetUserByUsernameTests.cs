using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Contract.Responses;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Users
{
    public class GetUserByUsernameTests : ControllerTestsBase
    {
        [Test]
        public async Task Should_get_user_by_username()
        {
            var user = await Context.Data.SeedUser();

            var uri = ApiUriFactory.UserEndpoints.GetUserByUsername(user.Username);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, user.UserType);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_username()
        {
            const string USERNAME = "made_up_username@email.com";

            var uri = ApiUriFactory.UserEndpoints.GetUserByUsername(USERNAME);
            await SendGetRequest(uri);
            VerifyResponse(HttpStatusCode.NotFound, false);
        }

        [Test]
        public async Task Should_get_user_regardless_of_case()
        {
            var user = await Context.Data.SeedUser();

            var uri = ApiUriFactory.UserEndpoints.GetUserByUsername(user.Username.ToUpperInvariant());
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, user.UserType);
        }
    }
}
