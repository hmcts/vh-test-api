using System;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;
using VideoApi.Contract.Responses;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class GetConferenceByHearingRefIdTests : ConferencesTestsBase
    {
        [Test]
        public async Task Should_get_conference_by_hearing_ref_id()
        {
            var request = CreateConferenceRequest();
            var conference = await CreateConference(request);

            var uri = ApiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(conference.HearingId);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<ConferenceDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.ConferenceDetailsResponse(response, request);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing_id()
        {
            var uri = ApiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(Guid.NewGuid());
            await SendGetRequest(uri);
            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
