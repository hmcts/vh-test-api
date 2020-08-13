using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Queries;
using TestApi.Domain.Enums;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetAllUsersByUserTypeQueryTests : DatabaseTestsBase
    {
        private readonly GetAllUsersByUserTypeQueryHandler _query;

        public GetAllUsersByUserTypeQueryTests()
        {
            _query = new GetAllUsersByUserTypeQueryHandler(_dbContext);
        }

        [Test]
        public async Task Should_get_only_users_with_user_type()
        {
            var expectedUser = await _context.TestDataManager.SeedUser();
            var unexpectedUser = await _context.TestDataManager.SeedUser(UserType.None);
            var users = await _query.Handle(new GetAllUsersByUserTypeQuery(expectedUser.UserType,
                expectedUser.Application));
            users.Count.Should().Be(1);
            users.Any(x => x.DisplayName.Equals(unexpectedUser.DisplayName)).Should().BeFalse();
            users.First().Should().BeEquivalentTo(expectedUser);
        }

        [Test]
        public async Task Should_return_empty_list_for_no_suitable_users()
        {
            var users = await _query.Handle(new GetAllUsersByUserTypeQuery(UserType.None, Application.TestApi));
            users.Count.Should().Be(0);
        }
    }
}