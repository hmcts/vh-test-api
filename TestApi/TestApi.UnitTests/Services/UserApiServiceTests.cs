using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.UnitTests.Services
{
    public class UserApiServiceTests : ServicesTestBase
    {
        private readonly UserApiException _notFoundError = new UserApiException("User not found", 404, "Response",
            new Dictionary<string, IEnumerable<string>>(), new Exception("Message"));
        private readonly UserApiException _internalServerError = new UserApiException("Internal server error", 500, "Response",
            new Dictionary<string, IEnumerable<string>>(), new Exception("Message"));

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

            UserApiClient.Setup(x => x.GetUserByAdUserNameAsync(USERNAME)).ThrowsAsync(_notFoundError);

            var userExists = await UserApiService.CheckUserExistsInAAD(USERNAME);
            userExists.Should().BeFalse();
        }

        [Test]
        public async Task Should_throw_error_whilst_checking_if_user_exists()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            UserApiClient.Setup(x => x.GetUserByAdUserNameAsync(USERNAME))
                .ThrowsAsync(_internalServerError);

            try
            {
                await UserApiService.CheckUserExistsInAAD(USERNAME);
            }
            catch (UserApiException ex)
            {
                ex.StatusCode.Should().Be(_internalServerError.StatusCode);
            }
        }

        [TestCase(UserType.Judge)]
        [TestCase(UserType.Individual)]
        [TestCase(UserType.Representative)]
        [TestCase(UserType.VideoHearingsOfficer)]
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.Observer)]
        [TestCase(UserType.PanelMember)]
        [TestCase(UserType.Winger)]
        public async Task Should_create_new_user_in_aad(UserType userType)
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var userRequest = new UserBuilder(EMAIL_STEM, 1)
                .WithUserType(userType)
                .ForApplication(Application.TestApi)
                .BuildRequest();

            var newUserResponse = new NewUserResponse
            {
                One_time_password = "password",
                User_id = "1234",
                Username = userRequest.Username
            };

            UserApiClient.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(newUserResponse);
            UserApiClient.Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .Returns(Task.CompletedTask);

            var userDetails = await UserApiService.CreateNewUserInAAD(userRequest.FirstName, userRequest.LastName, userRequest.ContactEmail, userRequest.IsProdUser);
            userDetails.One_time_password.Should().Be(newUserResponse.One_time_password);
            userDetails.User_id.Should().Be(newUserResponse.User_id);
            userDetails.Username.Should().Be(newUserResponse.Username);
        }

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
                One_time_password = "password",
                User_id = "1234",
                Username = userRequest.Username
            };

            UserApiClient.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(newUserResponse);
            UserApiClient.Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .Returns(Task.CompletedTask);

            var userDetails = await UserApiService.CreateNewUserInAAD(userRequest.FirstName, userRequest.LastName, userRequest.ContactEmail, userRequest.IsProdUser);
            userDetails.One_time_password.Should().Be(newUserResponse.One_time_password);
            userDetails.User_id.Should().Be(newUserResponse.User_id);
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
                One_time_password = "password",
                User_id = "1234",
                Username = userRequest.Username
            };

            UserApiClient.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(newUserResponse);
            UserApiClient.Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .Returns(Task.CompletedTask);

            var userDetails = await UserApiService.CreateNewUserInAAD(userRequest.FirstName, userRequest.LastName, userRequest.ContactEmail, userRequest.IsProdUser);
            userDetails.One_time_password.Should().Be(newUserResponse.One_time_password);
            userDetails.User_id.Should().Be(newUserResponse.User_id);
            userDetails.Username.Should().Be(newUserResponse.Username);
        }

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
                .ThrowsAsync(_internalServerError);

            try
            {
                await UserApiService.DeleteUserInAAD(USERNAME);
            }
            catch (UserApiException ex)
            {
                ex.StatusCode.Should().Be(_internalServerError.StatusCode);
            }
        }

        [Test]
        public async Task Should_add_prod_judge_groups()
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var prodJudge = new UserBuilder(EMAIL_STEM, 1)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .IsProdUser(true)
                .BuildUser();

            var nonProdJudge = new UserBuilder(EMAIL_STEM, 2)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .IsProdUser(false)
                .BuildUser();

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
                .BuildUser();

            var nonPerformanceTestUser = new UserBuilder(EMAIL_STEM, 2)
                .ForTestType(TestType.Automated)
                .WithUserType(UserType.Individual)
                .BuildUser();

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
        [TestCase(UserType.Winger)]
        public async Task Should_add_user_to_groups(UserType userType)
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var user = new UserBuilder(EMAIL_STEM, 1)
                .ForTestType(TestType.Automated)
                .WithUserType(userType)
                .BuildUser();

            UserApiClient.Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .Returns(Task.CompletedTask);

            var groupsCount = await UserApiService.AddGroupsToUser(user, "1");
            groupsCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task Should_throw_error_if_failed_to_add_user_to_group()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            UserApiClient
                .Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>()))
                .ThrowsAsync(_internalServerError);
            try
            {
                await UserApiService.DeleteUserInAAD(USERNAME);
            }
            catch (UserApiException ex)
            {
                ex.StatusCode.Should().Be(_internalServerError.StatusCode);
            }
        }
    }
}