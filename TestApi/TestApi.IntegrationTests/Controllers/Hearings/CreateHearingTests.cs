using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Tests.Common;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class CreateHearingTests : HearingsTestsBase
    {
        [TestCase(TestType.Automated)]
        [TestCase(TestType.ITHC)]
        [TestCase(TestType.Manual)]
        [TestCase(TestType.Performance)]
        public async Task Should_create_hearing(TestType testType)
        {
            var request = CreateHearingRequest(testType);
            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
        }
    }
}
