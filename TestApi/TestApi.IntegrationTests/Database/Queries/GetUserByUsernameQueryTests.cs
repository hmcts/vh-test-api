using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetUserByUsernameQueryTests : DatabaseTestsBase
    {
        private readonly GetUserByUsernameQueryHandler _query;

        public GetUserByUsernameQueryTests()
        {
            _query = new GetUserByUsernameQueryHandler(_dbContext);
        }

        [Test]
        public async Task Should_get_user_by_username()
        {
            var user = await _context.TestDataManager.SeedUser();
            var userDetails = await _query.Handle(new GetUserByUsernameQuery(user.Username));
            userDetails.Should().BeEquivalentTo(user);
        }

        [Test]
        public void Should_throw_user_not_found_for_nonexistent_user_id()
        {
            Assert.ThrowsAsync<UserNotFoundException>(() => _query.Handle(
                new GetUserByUsernameQuery("Made up username")));
        }
    }
}
