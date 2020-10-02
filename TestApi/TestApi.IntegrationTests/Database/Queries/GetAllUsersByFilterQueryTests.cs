using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Queries;
using TestApi.Domain.Enums;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetAllUsersByFilterQueryTests : DatabaseTestsBase
    {
        private readonly GetAllUsersByFilterQueryHandler _query;

        public GetAllUsersByFilterQueryTests()
        {
            _query = new GetAllUsersByFilterQueryHandler(DbContext);
        }

        [Test]
        public async Task Should_get_only_users_with_user_type()
        {
            var expectedUser = await Context.Data.SeedUser();
            var unexpectedUser = await Context.Data.SeedUser(UserType.None);
            var users = await _query.Handle(new GetAllUsersByFilterQuery(expectedUser.UserType, expectedUser.TestType,
                expectedUser.Application, expectedUser.IsProdUser));
            users.Count.Should().Be(1);
            users.Any(x => x.DisplayName.Equals(unexpectedUser.DisplayName)).Should().BeFalse();
            users.First().Should().BeEquivalentTo(expectedUser);
        }

        [Test]
        public async Task Should_return_empty_list_for_no_suitable_users()
        {
            var users = await _query.Handle(new GetAllUsersByFilterQuery(UserType.None, TestType.Automated, Application.TestApi, false));
            users.Count.Should().Be(0);
        }
    }
}