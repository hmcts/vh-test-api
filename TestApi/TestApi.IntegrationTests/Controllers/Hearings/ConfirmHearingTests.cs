using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Domain.Enums;
using TestApi.Services.Builders.Requests;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class ConfirmHearingTests : HearingsTestsBase
    {
        [Test]
        public async Task Should_confirm_hearing()
        {
            var hearingRequest = CreateHearingRequest();
            var hearingResponse = await CreateHearing(hearingRequest);
            var caseAdmin = hearingRequest.Users.First(x => x.UserType == UserType.CaseAdmin);

            var request = new UpdateBookingRequestBuilder().UpdatedBy(caseAdmin.Username).Build();

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
            const string CASE_ADMIN_USERNAME = EmailData.NON_EXISTENT_USERNAME;

            var request = new UpdateBookingRequestBuilder()
                .UpdatedBy(CASE_ADMIN_USERNAME)
                .Build();

            var uri = ApiUriFactory.HearingEndpoints.ConfirmHearing(Guid.NewGuid());
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
