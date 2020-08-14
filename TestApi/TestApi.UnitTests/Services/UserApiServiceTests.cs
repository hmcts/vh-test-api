using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.UnitTests.Services
{
    public class UserApiServiceTests : ServicesTestBase
    {
        [Test]
        public async Task Should_return_true_for_existing_user_in_aad()
        {
            const string CONTACT_EMAIL = "made_up_email_address@email.com";

            UserApiClient
                .Setup(x => x.GetUserByEmailAsync(CONTACT_EMAIL)).ReturnsAsync(It.IsAny<UserProfile>());

            var userExists = await UserApiService.CheckUserExistsInAAD(CONTACT_EMAIL);
            userExists.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_false_for_nonexistent_user_in_aad()
        {
            const string CONTACT_EMAIL = "made_up_email_address@email.com";

            var ex = new UserApiException("User not found", 404, "Response",
                new Dictionary<string, IEnumerable<string>>(), new Exception("Message"));

            UserApiClient.Setup(x => x.GetUserByEmailAsync(CONTACT_EMAIL)).ThrowsAsync(ex);

            var userExists = await UserApiService.CheckUserExistsInAAD(CONTACT_EMAIL);
            userExists.Should().BeFalse();
        }

        [Test]
        public async Task Should_create_new_user_in_aad()
        {
            const string EMAIL_STEM = "made_up_email_stem.com";

            var userRequest = new UserBuilder(EMAIL_STEM, 1)
                .WithUserType(UserType.Individual)
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

            var userDetails = await UserApiService.CreateNewUserInAAD(userRequest.FirstName, userRequest.LastName, userRequest.ContactEmail);
            userDetails.One_time_password.Should().Be(newUserResponse.One_time_password);
            userDetails.User_id.Should().Be(newUserResponse.User_id);
            userDetails.Username.Should().Be(newUserResponse.Username);
        }
    }
}