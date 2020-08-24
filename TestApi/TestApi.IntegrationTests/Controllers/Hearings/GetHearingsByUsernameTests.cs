using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class GetHearingsByUsernameTests : HearingsTestsBase
    {
        [Test]
        public async Task Should_return_hearings_by_username()
        {
            var request = CreateHearingRequest();
            var hearingResponse = await CreateHearing(request);
            var participant = hearingResponse.Participants.First();

            var uri = ApiUriFactory.HearingEndpoints.GetHearingsByUsername(participant.Username);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<HearingDetailsResponse>>(Json);

            response.Should().NotBeNull();
            response.Count.Should().Be(1);
            Verify.HearingDetailsResponse(response.First(), request);
        }

        [Test]
        public async Task Should_return_empty_list_for_non_existent_username()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            var uri = ApiUriFactory.HearingEndpoints.GetHearingsByUsername(USERNAME);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<HearingDetailsResponse>>(Json);

            response.Should().NotBeNull();
            response.Count.Should().Be(0);
        }
    }
}
