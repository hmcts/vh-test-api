using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.UnitTests.Services.UserApiService
{
    public class CheckUserExistsServiceTests : ServicesTestBase
    {
        [Test]
        public async Task Should_return_true_for_existing_user_in_aad()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            UserApiClient
                .Setup(x => x.GetUserByAdUserNameAsync(USERNAME)).ReturnsAsync(It.IsAny<UserProfile>());

            var userExists = await UserApiService.CheckUserExistsInAAD(USERNAME);
            userExists.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_false_for_nonexistent_user_in_aad()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            UserApiClient.Setup(x => x.GetUserByAdUserNameAsync(USERNAME)).ThrowsAsync(NotFoundError);

            var userExists = await UserApiService.CheckUserExistsInAAD(USERNAME);
            userExists.Should().BeFalse();
        }

        [Test]
        public async Task Should_throw_error_whilst_checking_if_user_exists()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            UserApiClient.Setup(x => x.GetUserByAdUserNameAsync(USERNAME))
                .ThrowsAsync(InternalServerError);

            try
            {
                await UserApiService.CheckUserExistsInAAD(USERNAME);
            }
            catch (UserApiException ex)
            {
                ex.StatusCode.Should().Be(InternalServerError.StatusCode);
            }
        }
    }
}
