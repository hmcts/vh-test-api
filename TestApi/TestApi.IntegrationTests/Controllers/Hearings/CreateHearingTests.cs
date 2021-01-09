using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
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

        [Test]
        public async Task Should_create_CACD_hearing()
        {
            var request = CreateCACDHearingRequest();
            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
        }

        [Test]
        public async Task Should_create_hearing_with_several_endpoints()
        {
            const int ENDPOINTS_COUNT = 2;
            var request = CreateHearingRequest();
            request.Endpoints = ENDPOINTS_COUNT;

            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
            response.Endpoints.Count.Should().Be(ENDPOINTS_COUNT);
            response.Endpoints.All(x => x.Display_name.StartsWith(HearingData.ENDPOINT_PREFIX)).Should().BeTrue();
        }
    }
}
