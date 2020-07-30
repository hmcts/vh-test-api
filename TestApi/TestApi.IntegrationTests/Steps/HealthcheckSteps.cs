using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestApi.Contract.Responses;
using TestApi.IntegrationTests.Configuration;

namespace TestApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class HealthCheckSteps : BaseSteps
    {
        private readonly TestContext _context;
        private HealthCheckResponse _response;

        public HealthCheckSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get health request")]
        public void GivenIMakeACallToTheHealthCheckEndpoint()
        {
            _context.Uri = ApiUriFactory.HealthCheckEndpoints.CheckServiceHealth;
            _context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the application version should be retrieved")]
        public async Task ThenTheApplicationVersionShouldBeRetrieved()
        {
            var json = await _context.Response.Content.ReadAsStringAsync();
            _response = RequestHelper.DeserialiseSnakeCaseJsonToResponse<HealthCheckResponse>(json);
            _response.Version.Version.Should().NotBeNull();
            _response.ErrorMessage.Should().BeNullOrWhiteSpace();
            _response.Successful.Should().BeTrue();
        }

        [Then(@"the user api should be available")]
        public void ThenTheUserApiShouldBeAvailable()
        {
            _response.UserApiHealth.Successful.Should().BeTrue();
            _response.UserApiHealth.ErrorMessage.Should().BeNullOrWhiteSpace();
        }
    }
}
