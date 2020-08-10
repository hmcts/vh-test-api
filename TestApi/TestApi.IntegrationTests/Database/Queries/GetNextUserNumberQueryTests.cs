using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Queries;
using TestApi.Domain.Enums;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetNextUserNumberQueryTests : DatabaseTestsBase
    {
        private readonly GetNextUserNumberByUserTypeQueryHandler _query;

        public GetNextUserNumberQueryTests()
        {
            _query = new GetNextUserNumberByUserTypeQueryHandler(_dbContext);
        }

        [Test]
        public async Task Should_get_next_user_number_by_user_type()
        {
            var user = await _context.TestDataManager.SeedUser();

            var nextNumber = await _query.Handle(new GetNextUserNumberByUserTypeQuery(user.UserType, user.Application));
            nextNumber.Value.Should().Be(2);
        }

        [Test]
        public async Task Should_return_one_if_no_applicable_users_exist()
        {
            var nextNumber = await _query.Handle(new GetNextUserNumberByUserTypeQuery(UserType.None, Application.TestApi));
            nextNumber.Value.Should().Be(1);
        }

        [Test]
        public async Task Should_return_one_if_users_exist_but_for_wrong_type()
        {
            await _context.TestDataManager.SeedUser();
            var nextNumber = await _query.Handle(new GetNextUserNumberByUserTypeQuery(UserType.None, Application.TestApi));
            nextNumber.Value.Should().Be(1);
        }

        [Test]
        public async Task Should_get_next_user_number_if_multiple_users_with_same_user_type_exist()
        {
            const UserType userType = UserType.None; 
            await _context.TestDataManager.SeedUser(userType);
            await _context.TestDataManager.SeedUser(userType);
            await _context.TestDataManager.SeedUser(userType);

            var nextNumber = await _query.Handle(new GetNextUserNumberByUserTypeQuery(userType, Application.TestApi));
            nextNumber.Value.Should().Be(4);
        }
    }
}
