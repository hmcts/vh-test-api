using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Enums;
using TestApi.Mappings;

namespace TestApi.UnitTests.Mappings
{
    public class UserToUserDetailsMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var user = new UserBuilder(EmailData.FAKE_EMAIL_STEM, 1)
                .WithUserType(UserType.Individual)
                .ForApplication(Application.TestApi)
                .BuildUserDto();

            var response = UserToDetailsResponseMapper.MapToResponse(user);
            response.Should().BeEquivalentTo(user);
        }
    }
}