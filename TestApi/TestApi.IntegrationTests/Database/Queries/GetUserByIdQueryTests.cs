using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Queries;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetUserByIdQueryTests : DatabaseTestsBase
    {
        private readonly GetUserByIdQueryHandler _query;

        public GetUserByIdQueryTests()
        {
            _query = new GetUserByIdQueryHandler(_dbContext);
        }

        [Test]
        public async Task Should_get_user_by_id()
        {
            var user = await _context.TestDataManager.SeedUser();
            var userDetails = await _query.Handle(new GetUserByIdQuery(user.Id));
            userDetails.Should().BeEquivalentTo(user);
        }

        [Test]
        public async Task Should_not_throw_error_for_nonexistent_user_id()
        {
            var madeUpUserId = Guid.NewGuid();
            var userDetails = await _query.Handle(new GetUserByIdQuery(madeUpUserId));
            userDetails.Should().BeNull();
        }
    }
}
