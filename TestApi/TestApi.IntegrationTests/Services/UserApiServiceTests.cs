using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Contracts;

namespace TestApi.IntegrationTests.Services
{
    public class UserApiServiceTests : ServicesTestBase
    {
        private readonly IUserApiService _userApiService;
        private readonly Mock<IUserApiClient> _apiClient;

        public UserApiServiceTests()
        {
            _apiClient = new Mock<IUserApiClient>();
            _userApiService = new UserApiService(_apiClient.Object, _context.Config.UserGroupsConfig);
        }

        [Test]
        public async Task Should_return_true_for_existing_user_in_aad()
        {
            const string CONTACT_EMAIL = "made_up_email_address@email.com";

            _apiClient.Setup(x => x.GetUserByEmailAsync(CONTACT_EMAIL)).ReturnsAsync(It.IsAny<UserProfile>());

            var userExists = await _userApiService.CheckUserExistsInAAD(CONTACT_EMAIL);
            userExists.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_false_for_nonexistent_user_in_aad()
        {
            const string CONTACT_EMAIL = "made_up_email_address@email.com";
            
            var ex = new UserApiException("User not found", 404, "Response", new Dictionary<string, IEnumerable<string>>(), new Exception("Message"));

            _apiClient.Setup(x => x.GetUserByEmailAsync(CONTACT_EMAIL)).ThrowsAsync(ex);

            var userExists = await _userApiService.CheckUserExistsInAAD(CONTACT_EMAIL);
            userExists.Should().BeFalse();
        }

        [Test]
        public async Task Should_create_new_user_in_aad()
        {
            var userRequest = new UserBuilder(_context.Config.UsernameStem, 1)
                .WithUserType(UserType.Individual)
                .ForApplication(Application.TestApi)
                .BuildRequest();

            var adUser = new ADUserBuilder(userRequest).BuildUser();

            var newUserResponse = new NewUserResponse()
            {
                One_time_password = "password",
                User_id = "1234",
                Username = userRequest.Username
            };

            _apiClient.Setup(x => x.CreateUserAsync(It.IsAny<CreateUserRequest>())).ReturnsAsync(newUserResponse);
            _apiClient.Setup(x => x.AddUserToGroupAsync(It.IsAny<AddUserToGroupRequest>())).Returns(Task.CompletedTask);

            var userDetails = await _userApiService.CreateNewUserInAAD(adUser);
            userDetails.One_time_password.Should().Be(newUserResponse.One_time_password);
            userDetails.User_id.Should().Be(newUserResponse.User_id);
            userDetails.Username.Should().Be(newUserResponse.Username);
        }
    }
}
