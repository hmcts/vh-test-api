using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Contract.Requests;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Utilities
{
    public class DeleteHearingsWithUtilityTests : ControllerTestsBase
    {
        private const string NAME_THAT_WONT_BE_FOUND = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [Test]
        public async Task Should_remove_hearings()
        {
            var request = new DeleteTestHearingDataRequest()
            {
                PartialHearingCaseNumber = $"Test {NAME_THAT_WONT_BE_FOUND}",
                PartialHearingCaseName = string.Empty,
                Limit = 1
            };

            var uri = ApiUriFactory.UtilityEndpoints.DeleteHearings;
            await SendPostRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<int>(Json);

            response.Should().Be(0);
        }

        [Test]
        public async Task Should_throw_error_without_specifying_test_in_request()
        {
            var request = new DeleteTestHearingDataRequest()
            {
                PartialHearingCaseNumber = NAME_THAT_WONT_BE_FOUND,
                PartialHearingCaseName = string.Empty,
                Limit = 1
            };

            var uri = ApiUriFactory.UtilityEndpoints.DeleteHearings;
            await SendPostRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.BadRequest, false);
        }
    }
}
