using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TestApi.UnitTests.Controllers.Users
{
    public class RefreshJudgesCacheControllerTests : UserControllerTestsBase
    {
        [Test]
        public async Task Should_refresh_judges_cache()
        {
            UserApiClient
                .Setup(x => x.RefreshJudgeCacheAsync())
                .Returns(Task.CompletedTask);

            var result = await Controller.RefreshJudgesCache();

            result.Should().NotBeNull();
            var objectResult = (OkResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }
    }
}