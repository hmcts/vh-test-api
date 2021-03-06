﻿using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Enums;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace TestApi.UnitTests.Services.UserApiService
{
    public class AddUserToGroupsServiceTests : ServicesTestBase
    {
        [Test]
        public async Task Should_add_prod_groups_to_prod_user()
        {
            const bool IS_PROD_USER = UserData.IS_PROD_USER;
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var userRequest = new UserBuilder(EMAIL_STEM, 1)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .IsProdUser(IS_PROD_USER)
                .BuildRequest();

            var newUserResponse = new NewUserResponse
            {
                OneTimePassword = "password",
                UserId = "1234",
                Username = userRequest.Username
            };

            UserApiClient.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(newUserResponse);
            UserApiClient.Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .Returns(Task.CompletedTask);

            var userDetails = await UserApiService.CreateNewUserInAAD(userRequest.FirstName, userRequest.LastName, userRequest.ContactEmail, userRequest.IsProdUser);
            userDetails.OneTimePassword.Should().Be(newUserResponse.OneTimePassword);
            userDetails.UserId.Should().Be(newUserResponse.UserId);
            userDetails.Username.Should().Be(newUserResponse.Username);
        }

        [Test]
        public async Task Should_add_performance_test_groups_to_performance_test_user()
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var userRequest = new UserBuilder(EMAIL_STEM, 1)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .ForTestType(TestType.Performance)
                .BuildRequest();

            var newUserResponse = new NewUserResponse
            {
                OneTimePassword = "password",
                UserId = "1234",
                Username = userRequest.Username
            };

            UserApiClient.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(newUserResponse);
            UserApiClient.Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .Returns(Task.CompletedTask);

            var userDetails = await UserApiService.CreateNewUserInAAD(userRequest.FirstName, userRequest.LastName, userRequest.ContactEmail, userRequest.IsProdUser);
            userDetails.OneTimePassword.Should().Be(newUserResponse.OneTimePassword);
            userDetails.UserId.Should().Be(newUserResponse.UserId);
            userDetails.Username.Should().Be(newUserResponse.Username);
        }

        [Test]
        public async Task Should_add_prod_judge_groups()
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var prodJudge = new UserBuilder(EMAIL_STEM, 1)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .IsProdUser(true)
                .BuildUserDto();

            var nonProdJudge = new UserBuilder(EMAIL_STEM, 2)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .IsProdUser(false)
                .BuildUserDto();

            UserApiClient.Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .Returns(Task.CompletedTask);

            var groupsForProdCount = await UserApiService.AddGroupsToUser(prodJudge, "1");
            var groupsForNonProdCount = await UserApiService.AddGroupsToUser(nonProdJudge, "2");

            groupsForProdCount.Should().BeGreaterThan(groupsForNonProdCount);
        }

        [Test]
        public async Task Should_add_performance_test_groups()
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var performanceTestUser = new UserBuilder(EMAIL_STEM, 1)
                .ForTestType(TestType.Performance)
                .WithUserType(UserType.Individual)
                .BuildUserDto();

            var nonPerformanceTestUser = new UserBuilder(EMAIL_STEM, 2)
                .ForTestType(TestType.Automated)
                .WithUserType(UserType.Individual)
                .BuildUserDto();

            UserApiClient.Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .Returns(Task.CompletedTask);

            var groupsForPerformanceTestUserCount = await UserApiService.AddGroupsToUser(performanceTestUser, "1");
            var groupsForNonPerformanceTestUserCount = await UserApiService.AddGroupsToUser(nonPerformanceTestUser, "2");

            groupsForPerformanceTestUserCount.Should().BeGreaterThan(groupsForNonPerformanceTestUserCount);
        }

        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.Individual)]
        [TestCase(UserType.Judge)]
        [TestCase(UserType.Observer)]
        [TestCase(UserType.PanelMember)]
        [TestCase(UserType.Representative)]
        [TestCase(UserType.Tester)]
        [TestCase(UserType.VideoHearingsOfficer)]
        [TestCase(UserType.Winger)]
        [TestCase(UserType.Witness)]
        [TestCase(UserType.Interpreter)]
        public async Task Should_add_user_to_groups_by_user_type(UserType userType)
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var user = new UserBuilder(EMAIL_STEM, 1)
                .ForTestType(TestType.Automated)
                .WithUserType(userType)
                .BuildUserDto();

            UserApiClient.Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .Returns(Task.CompletedTask);

            var groupsCount = await UserApiService.AddGroupsToUser(user, "1");
            groupsCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task Should_throw_error_if_failed_to_add_user_to_group()
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var user = new UserBuilder(EMAIL_STEM, 1)
                .ForTestType(TestType.Automated)
                .WithUserType(UserType.Individual)
                .BuildUserDto();

            UserApiClient
                .Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .ThrowsAsync(InternalServerError);
            try
            {
                await UserApiService.AddGroupsToUser(user, "1");
            }
            catch (UserApiException ex)
            {
                ex.StatusCode.Should().Be(InternalServerError.StatusCode);
            }
        }
    }
}