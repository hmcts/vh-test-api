using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.UnitTests.Services.UserApiService
{
    public class CreateNewUserServiceTests : ServicesTestBase
    {
        [TestCase(UserType.Judge)]
        [TestCase(UserType.Individual)]
        [TestCase(UserType.Representative)]
        [TestCase(UserType.VideoHearingsOfficer)]
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.Observer)]
        [TestCase(UserType.PanelMember)]
        [TestCase(UserType.Winger)]
        [TestCase(UserType.Tester)]
        [TestCase(UserType.Witness)]
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
    }
}
