using System;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.IntegrationTests.Configuration;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class GetHearingTests : HearingsTestsBase
    {
        [Test]
        public async Task Should_get_hearing_by_id()
        {
            var request = CreateHearingRequest();
            var hearingResponse = await CreateHearing(request);

            var uri = ApiUriFactory.HearingEndpoints.GetHearing(hearingResponse.Id);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing_id()
        {
            var uri = ApiUriFactory.HearingEndpoints.GetHearing(Guid.NewGuid());
            await SendGetRequest(uri);
            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
