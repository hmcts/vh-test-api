using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Contract.Responses;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Health
{
    public class HealthTests : ControllerTestsBase
    {
        [Test]
        [Category("Health")]
        public async Task Should_return_OK()
        {
            var uri = ApiUriFactory.HealthCheckEndpoints.CheckServiceHealth;
            await SendGetRequest(uri);
            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<HealthResponse>(Json);

            response.BookingsApiHealth.Successful.Should().BeTrue();
            response.BookingsApiHealth.Data.Should().BeNull();
            response.BookingsApiHealth.ErrorMessage.Should().BeNullOrWhiteSpace();

            response.TestApiHealth.Successful.Should().BeTrue();
            response.TestApiHealth.Data.Should().BeNull();
            response.TestApiHealth.ErrorMessage.Should().BeNullOrWhiteSpace();

            response.UserApiHealth.Successful.Should().BeTrue();
            response.UserApiHealth.Data.Should().BeNull();
            response.UserApiHealth.ErrorMessage.Should().BeNullOrWhiteSpace();

            response.VideoApiHealth.Successful.Should().BeTrue();
            response.VideoApiHealth.Data.Should().BeNull();
            response.VideoApiHealth.ErrorMessage.Should().BeNullOrWhiteSpace();

            response.Version.Should().NotBeNull();
            response.Version.Version.Should().NotBeNull();
            response.Version.FileVersion.Should().NotBeNull();
            response.Version.InformationVersion.Should().NotBeNull();
        }
    }
}
