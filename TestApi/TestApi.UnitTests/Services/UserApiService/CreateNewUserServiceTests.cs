using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Enums;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

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
        [TestCase(UserType.Interpreter)]
        public async Task Should_create_new_user_in_aad(UserType userType)
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var userRequest = new UserBuilder(EMAIL_STEM, 1)
                .WithUserType(userType)
                .ForApplication(Application.TestApi)
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
    }
}
