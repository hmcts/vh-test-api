using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class DeleteHearingTests : HearingsTestsBase
    {
        [Test]
        public async Task Should_delete_hearing_by_id()
        {
            var request = CreateHearingRequest();
            var hearingResponse = await CreateHearing(request);

            await DeleteHearing(hearingResponse.Id);

            VerifyResponse(HttpStatusCode.NoContent, true);
            HearingsToDelete.Remove(hearingResponse);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing_id()
        {
            var uri = ApiUriFactory.HearingEndpoints.DeleteHearing(Guid.NewGuid());
            await SendGetRequest(uri);
            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
