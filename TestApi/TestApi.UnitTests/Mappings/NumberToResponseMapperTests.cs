using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Helpers;
using TestApi.Mappings;

namespace TestApi.UnitTests.Mappings
{
    public class NumberToResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var number = new Integer(1);
            var response = NumberToResponseMapper.MapToResponse(number);
            response.Number.Should().Be(number);
        }
    }
}
