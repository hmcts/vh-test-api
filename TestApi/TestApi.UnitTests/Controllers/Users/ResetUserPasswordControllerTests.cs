using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace TestApi.UnitTests.Controllers.Users
{
    public class ResetUserPasswordControllerTests : UserControllerTestsBase
    {
        [Test]
        public async Task Should_reset_user_password()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;
            const string NEW_PASSWORD = UserData.NEW_PASSWORD;

            var request = new ResetUserPasswordRequest()
            {
                Username = USERNAME
            };

            var response = new UpdateUserResponse()
            {
                NewPassword = NEW_PASSWORD
            };

            UserApiClient
                .Setup(x => x.ResetUserPasswordAsync(USERNAME))
                .ReturnsAsync(response);

            var result = await Controller.ResetUserPassword(request);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var passwordResponse = (UpdateUserResponse)objectResult.Value;
            passwordResponse.NewPassword.Should().Be(response.NewPassword);
        }

        [Test]
        public async Task Should_return_test_password_for_ejud_users()
        {
            var username = EjudUserData.USERNAME(EjudUserData.AUTOMATED_FIRST_NAME_PREFIX, EjudUserData.LAST_NAME(1), EjudUserData.FAKE_EJUD_DOMAIN);
            var request = new ResetUserPasswordRequest()
            {
                Username = username
            };

            var result = await Controller.ResetUserPassword(request);
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var passwordResponse = (UpdateUserResponse)objectResult.Value;
            passwordResponse.NewPassword.Should().Be(EjudUserData.FAKE_PASSWORD);
        }

        [Test]
        public void Should_throw_error_if_ejud_domain_not_found()
        {
            var username = EjudUserData.USERNAME(EjudUserData.AUTOMATED_FIRST_NAME_PREFIX, EjudUserData.LAST_NAME(1), EjudUserData.FAKE_EJUD_DOMAIN);
            var request = new ResetUserPasswordRequest()
            {
                Username = username
            };

            Configuration
                .Setup(x => x.GetSection("EjudUsernameStem").Value)
                .Returns(() => null);

            Assert.ThrowsAsync<ConfigurationErrorsException>(async () => await Controller.ResetUserPassword(request));
        }


        [Test]
        public void Should_throw_error_if_test_default_password_not_found()
        {
            var username = EjudUserData.USERNAME(EjudUserData.AUTOMATED_FIRST_NAME_PREFIX, EjudUserData.LAST_NAME(1), EjudUserData.FAKE_EJUD_DOMAIN);
            var request = new ResetUserPasswordRequest()
            {
                Username = username
            };

            Configuration
                .Setup(x => x.GetSection("TestDefaultPassword").Value)
                .Returns(() => null);

            Assert.ThrowsAsync<ConfigurationErrorsException>(async () => await Controller.ResetUserPassword(request));
        }
    }
}
