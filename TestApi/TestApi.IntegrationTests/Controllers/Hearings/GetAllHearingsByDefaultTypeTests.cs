using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class GetAllHearingsByDefaultTypeTests : HearingsTestsBase
    {
        [Test]
        public async Task Should_return_hearings_by_default_case_type()
        {
            var request = CreateHearingRequest();
            var hearingResponse = await CreateHearing(request);

            var uri = ApiUriFactory.HearingEndpoints.GetAllHearingsByDefaultCaseType();
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<BookingsHearingResponse>>(Json);

            response.Should().NotBeNull();
            response.Count.Should().BeGreaterThan(0);

            response.Any(x => x.HearingName.Equals(hearingResponse.Cases.Single().Name)).Should().BeTrue();
        }
    }
}
