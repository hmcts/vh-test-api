using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Enums;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.UnitTests.Controllers.Users
{
    public class GetUserByUserPrincipalNameControllerTests : UserControllerTestsBase
    {
        [Test]
        public async Task Should_get_existing_user_and_return_true()
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;
            const int NUMBER = 1;

            var user = new UserBuilder(EMAIL_STEM, NUMBER)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .BuildUserDto();

            var userProfile = new UserProfile()
            {
                Case_type = new List<string>(),
                Display_name = user.DisplayName,
                Email = user.ContactEmail,
                First_name = user.FirstName,
                Last_name = user.LastName,
                User_id = user.Id.ToString(),
                User_name = user.Username,
                User_role = user.UserType.ToString()
            };

            UserApiClient
                .Setup(x => x.GetUserByAdUserNameAsync(It.IsAny<string>()))
                .ReturnsAsync(userProfile);

            var result = await Controller.GetUserByUserPrincipleNameAsync(user.ContactEmail);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var userDetails = (UserProfile)objectResult.Value;
            userDetails.Should().BeEquivalentTo(userProfile);
        }

        [Test]
        public async Task Should_return_false_not_found_for_non_existent_user()
        {
            const string CONTACT_EMAIL = EmailData.NON_EXISTENT_CONTACT_EMAIL;

            var result = await Controller.GetUserExistsInAdAsync(CONTACT_EMAIL);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
