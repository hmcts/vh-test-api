﻿using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Services.Clients.UserApiClient;

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
                New_password = NEW_PASSWORD
            };

            UserApiClient
                .Setup(x => x.ResetUserPasswordAsync(USERNAME))
                .ReturnsAsync(response);

            var result = await Controller.ResetUserPasswordAsync(request);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var passwordResponse = (UpdateUserResponse)objectResult.Value;
            passwordResponse.New_password.Should().Be(response.New_password);
        }

        [Test]
        public async Task Should_return_not_found_for_unknown_username()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            var result = await Controller.GetUserDetailsByUsernameAsync(USERNAME);

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
