using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Users
{
    public class GetUserExistsInAdTests : ControllerTestsBase
    {
        [Test]
        public async Task Should_return_true_if_user_with_contact_email_exists()
        {
            var user = await Context.Data.SeedUser();

            var uri = ApiUriFactory.UserEndpoints.GetUserExistsInAd(user.ContactEmail);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<bool>(Json);

            response.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_false_not_found_for_non_existent_contact_email()
        {
            const string CONTACT_EMAIL = DefaultData.NON_EXISTENT_CONTACT_EMAIL;

            var uri = ApiUriFactory.UserEndpoints.GetUserExistsInAd(CONTACT_EMAIL);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.NotFound, false);
            var response = RequestHelper.Deserialise<bool>(Json);

            response.Should().BeFalse();
        }

        [Test]
        public async Task Should_get_existing_user_regardless_of_case()
        {
            var user = await Context.Data.SeedUser();

            var uri = ApiUriFactory.UserEndpoints.GetUserExistsInAd(user.ContactEmail.ToUpperInvariant());
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<bool>(Json);

            response.Should().BeTrue();
        }
    }
}
