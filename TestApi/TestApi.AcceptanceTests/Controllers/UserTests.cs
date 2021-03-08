using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Contract.Responses;
using TestApi.Contract.Enums;
using TestApi.Tests.Common.Configuration;

namespace TestApi.AcceptanceTests.Controllers
{
    public class UserTests : TestsBase
    {
        [Test]
        public void GetUserByUsername()
        {
            const UserType USER_TYPE = UserType.Individual;
            var userDetailsResponse = AllocateUser(USER_TYPE);
            var uri = ApiUriFactory.UserEndpoints.GetUserByUsername(userDetailsResponse.Username);

            var request = RequestHandler.Get(uri);
            var response = SendRequest(request);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccessful.Should().BeTrue();

            var getUserResponse = RequestHelper.Deserialise<UserDetailsResponse>(response.Content);
            getUserResponse.Should().BeEquivalentTo(userDetailsResponse);
        }
    }
}
