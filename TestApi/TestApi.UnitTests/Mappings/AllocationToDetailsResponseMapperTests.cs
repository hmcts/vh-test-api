using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Mappings;

namespace TestApi.UnitTests.Mappings
{
    public class AllocationToDetailsResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var user = new UserBuilder("made_up_email_stem_for_test", 1)
                .WithUserType(UserType.Individual)
                .ForApplication(Application.TestApi)
                .BuildUser();

            var allocation = new Allocation(user);

            var response = AllocationToDetailsResponseMapper.MapToResponse(allocation);
            response.Should().BeEquivalentTo(allocation, options => options.Excluding(x => x.User));
        }
    }
}
