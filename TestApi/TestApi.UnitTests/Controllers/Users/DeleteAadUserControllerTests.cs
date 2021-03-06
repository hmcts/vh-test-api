﻿using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace TestApi.UnitTests.Controllers.Users
{
    public class DeleteAadUserControllerTests : UserControllerTestsBase
    {
        [Test]
        public async Task Should_delete_hearing()
        {
            const string CONTACT_EMAIL = "aad_user@hmcts.net";

            UserApiService
                .Setup(x => x.CheckUserExistsInAAD(It.IsAny<string>()))
                .ReturnsAsync(true);

            UserApiService
                .Setup(x => x.DeleteUserInAAD(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var response = await Controller.DeleteADUser(CONTACT_EMAIL);
            response.Should().NotBeNull();

            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_email()
        {
            const string CONTACT_EMAIL = "aad_user@hmcts.net";

            UserApiService
                .Setup(x => x.CheckUserExistsInAAD(It.IsAny<string>()))
                .ReturnsAsync(false);

            var response = await Controller.DeleteADUser(CONTACT_EMAIL);
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
