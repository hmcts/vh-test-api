using System.Net;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Tests.Common.Configuration;

namespace TestApi.AcceptanceTests.Controllers
{
    public class HealthTests : TestsBase
    {
        [Test]
        public void GetHealth()
        {
            var request = RequestHandler.Get(ApiUriFactory.HealthCheckEndpoints.CheckServiceHealth);
            var response = SendRequest(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccessful.Should().BeTrue();
        }
    }
}
