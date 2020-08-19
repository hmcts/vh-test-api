using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class CreateConferenceTests : ConferencesTestsBase
    {
        [Test]
        public async Task Should_create_conference()
        {
            var request = CreateConferenceRequest();
            await CreateConference(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<ConferenceDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.ConferenceDetailsResponse(response, request);
        }
    }
}
