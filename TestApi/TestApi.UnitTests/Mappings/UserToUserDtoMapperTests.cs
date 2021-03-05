using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Common.Mappers;

namespace TestApi.UnitTests.Mappings
{
    public class UserToUserDtoMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var user = new UserBuilder(EmailData.FAKE_EMAIL_STEM, 1).BuildUser();
            var userDto = UserToUserDtoMapper.Map(user);
            userDto.Should().BeEquivalentTo(user);
        }
    }
}
