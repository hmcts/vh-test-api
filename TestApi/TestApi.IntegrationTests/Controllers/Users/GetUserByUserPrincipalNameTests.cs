using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;
using UserApi.Contract.Responses;

namespace TestApi.IntegrationTests.Controllers.Users
{
    public class GetUserByUserPrincipalNameTests : ControllerTestsBase
    {
        [Test]
        public async Task Should_get_user_by_username()
        {
            var user = await Context.Data.SeedUser();

            var uri = ApiUriFactory.UserEndpoints.GetUserByUserPrincipalName(user.Username);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserProfile>(Json);

            response.Should().NotBeNull();
            Verify.UserProfileResponse(response, user);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_username()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            var uri = ApiUriFactory.UserEndpoints.GetUserByUserPrincipalName(USERNAME);
            await SendGetRequest(uri);
            VerifyResponse(HttpStatusCode.NotFound, false);
        }

        [Test]
        public async Task Should_get_user_regardless_of_case()
        {
            var user = await Context.Data.SeedUser();

            var uri = ApiUriFactory.UserEndpoints.GetUserByUserPrincipalName(user.Username.ToUpperInvariant());
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserProfile>(Json);

            response.Should().NotBeNull();
            Verify.UserProfileResponse(response, user);
        }
    }
}
