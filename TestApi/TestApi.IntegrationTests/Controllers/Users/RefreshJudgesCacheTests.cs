using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Users
{
    public class RefreshJudgesCacheTests : ControllerTestsBase
    {
        [Test]
        public async Task Should_refresh_judges_cache()
        {
            var uri = ApiUriFactory.UserEndpoints.RefreshJudgesCache();
            await SendGetRequest(uri);
            VerifyResponse(HttpStatusCode.OK, true);
        }
    }
}
