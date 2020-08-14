using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class CreateHearingTests : HearingsTestsBase
    {
        [Test]
        public async Task Should_create_hearing()
        {
            var request = CreateHearingRequest();
            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
        }
    }
}
