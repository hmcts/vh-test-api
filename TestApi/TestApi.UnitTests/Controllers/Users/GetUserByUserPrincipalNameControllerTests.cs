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
using UserApi.Contract.Responses;

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
                CaseType = new List<string>(),
                DisplayName = user.DisplayName,
                Email = user.ContactEmail,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.Id.ToString(),
                UserName = user.Username,
                UserRole = user.UserType.ToString()
            };

            UserApiClient
                .Setup(x => x.GetUserByAdUserNameAsync(It.IsAny<string>()))
                .ReturnsAsync(userProfile);

            var result = await Controller.GetUserByUserPrincipleName(user.ContactEmail);

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

            var result = await Controller.GetUserExistsInAd(CONTACT_EMAIL);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
