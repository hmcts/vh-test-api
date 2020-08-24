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
        [Test]
        public async Task Should_return_true_for_existing_user_in_aad()
        {
            const string CONTACT_EMAIL = EmailData.NON_EXISTENT_CONTACT_EMAIL;

            UserApiClient
                .Setup(x => x.GetUserByEmailAsync(CONTACT_EMAIL)).ReturnsAsync(It.IsAny<UserProfile>());

            var userExists = await UserApiService.CheckUserExistsInAAD(CONTACT_EMAIL);
            userExists.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_false_for_nonexistent_user_in_aad()
        {
            const string CONTACT_EMAIL = EmailData.NON_EXISTENT_CONTACT_EMAIL;

            var ex = new UserApiException("User not found", 404, "Response",
                new Dictionary<string, IEnumerable<string>>(), new Exception("Message"));

            UserApiClient.Setup(x => x.GetUserByEmailAsync(CONTACT_EMAIL)).ThrowsAsync(ex);

            var userExists = await UserApiService.CheckUserExistsInAAD(CONTACT_EMAIL);
            userExists.Should().BeFalse();
        }

        [TestCase(UserType.Judge)]
        [TestCase(UserType.Individual)]
        [TestCase(UserType.Representative)]
        [TestCase(UserType.VideoHearingsOfficer)]
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.Observer)]
        [TestCase(UserType.PanelMember)]
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
    }
}