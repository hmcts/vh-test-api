using System;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Services.Builders.Requests;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;
using VideoApi.Contract.Responses;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class ConfirmHearingTests : HearingsTestsBase
    {
        [Test]
        public async Task Should_confirm_hearing()
        {
            var hearingRequest = CreateHearingRequest();
            var hearingResponse = await CreateHearing(hearingRequest);

            var request = new UpdateBookingRequestBuilder().Build();

            var uri = ApiUriFactory.HearingEndpoints.ConfirmHearing(hearingResponse.Id);
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<ConferenceDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.ConferenceDetailsResponse(response, hearingResponse);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing_id()
        {
            var request = new UpdateBookingRequestBuilder().Build();

            var uri = ApiUriFactory.HearingEndpoints.ConfirmHearing(Guid.NewGuid());
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
