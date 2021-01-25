using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.UnitTests.Services.UserApiService
{
    public class DeleteUserServiceTests : ServicesTestBase
    {
        [Test]
        public async Task Should_delete_user_in_aad()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            UserApiClient
                .Setup(x => x.DeleteUserAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await UserApiService.DeleteUserInAAD(USERNAME);
        }

        [Test]
        public async Task Should_throw_error_if_failed_to_delete_user_in_aad()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            UserApiClient
                .Setup(x => x.DeleteUserAsync(It.IsAny<string>()))
                .ThrowsAsync(InternalServerError);

            try
            {
                await UserApiService.DeleteUserInAAD(USERNAME);
            }
            catch (UserApiException ex)
            {
                ex.StatusCode.Should().Be(InternalServerError.StatusCode);
            }
        }
    }
}
