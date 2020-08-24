﻿using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;

namespace TestApi.UnitTests.Controllers.Users
{
    public class GetUserExistsInAdControllerTests : UserControllerTestsBase
    {
        [Test]
        public async Task Should_get_existing_user_and_return_true()
        {
            const string CONTACT_EMAIL = DefaultData.EXISTING_CONTACT_EMAIL;

            UserApiService
                .Setup(x => x.CheckUserExistsInAAD(It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await Controller.GetUserExistsInAdAsync(CONTACT_EMAIL);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var userExists = (bool)objectResult.Value;
            userExists.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_false_not_found_for_non_existent_ser()
        {
            const string CONTACT_EMAIL = DefaultData.NON_EXISTENT_CONTACT_EMAIL;

            UserApiService
                .Setup(x => x.CheckUserExistsInAAD(It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await Controller.GetUserExistsInAdAsync(CONTACT_EMAIL);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var userExists = (bool)objectResult.Value;
            userExists.Should().BeFalse();
        }
    }
}
