using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Domain.Enums;
using TestApi.Mappings;

namespace TestApi.UnitTests.Mappings
{
    public class UserToUserDetailsMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var user = new UserBuilder("made_up_email_stem_for_test", 1)
                .WithUserType(UserType.Individual)
                .ForApplication(Application.TestApi)
                .BuildUser();

            var response = UserToDetailsResponseMapper.MapToResponse(user);
            response.Should().BeEquivalentTo(user);
        }
    }
}