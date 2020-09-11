using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.DAL.Queries;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetRecentUserByUsernameQueryTests : DatabaseTestsBase
    {
        private readonly GetRecentUserByUsernameQueryHandler _query;

        public GetRecentUserByUsernameQueryTests()
        {
            _query = new GetRecentUserByUsernameQueryHandler(DbContext);
        }

        [Test]
        public async Task Should_get_recent_user_by_username()
        {
            const string USERNAME = EmailData.RECENT_USER_USERNAME;

            var recentUser = await Context.Data.SeedRecentUser(USERNAME);

            var recentUserDetails = await _query.Handle(new GetRecentUserByUsernameQuery(recentUser.Username));
            recentUserDetails.Username.Should().Be(recentUser.Username);
            recentUserDetails.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            recentUserDetails.IsRecentlyCreated().Should().BeTrue();
        }

        [Test]
        public async Task Should_not_throw_error_if_recent_user_does_not_exist()
        {
            const string USERNAME = EmailData.RECENT_USER_USERNAME;

            var allocationDetails = await _query.Handle(new GetRecentUserByUsernameQuery(USERNAME));
            allocationDetails.Should().BeNull();
        }
    }
}
