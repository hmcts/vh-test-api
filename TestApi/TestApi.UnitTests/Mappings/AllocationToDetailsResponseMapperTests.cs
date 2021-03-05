using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Domain;
using TestApi.Contract.Enums;
using TestApi.Mappings;

namespace TestApi.UnitTests.Mappings
{
    public class AllocationToDetailsResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var user = new UserBuilder(EmailData.FAKE_EMAIL_STEM, 1)
                .WithUserType(UserType.Individual)
                .ForApplication(Application.TestApi)
                .BuildUser();

            var allocation = new Allocation(user);

            var response = AllocationToDetailsResponseMapper.MapToResponse(allocation);
            response.Should().BeEquivalentTo(allocation, options => options.Excluding(x => x.User));
        }
    }
}