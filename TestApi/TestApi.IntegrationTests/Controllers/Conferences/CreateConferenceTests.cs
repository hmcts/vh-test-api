using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class CreateConferenceTests : ConferencesTestsBase
    {
        [TestCase(TestType.Automated)]
        [TestCase(TestType.Demo)]
        [TestCase(TestType.ITHC)]
        [TestCase(TestType.Manual)]
        [TestCase(TestType.Performance)]
        public async Task Should_create_conference(TestType testType)
        {
            var request = CreateConferenceRequest(testType);
            await CreateConference(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<ConferenceDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.ConferenceDetailsResponse(response, request);
        }

        [Test]
        public async Task Should_create_cacd_conference()
        {
            var request = CreateCACDConferenceRequest();
            await CreateConference(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<ConferenceDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.ConferenceDetailsResponse(response, request);
        }
    }
}
