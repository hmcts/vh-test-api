using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.DAL.Commands;
using TestApi.DAL.Exceptions;

namespace TestApi.IntegrationTests.Database.Commands
{
    public class DeleteNewRecentUserByUsernameCommandTests : DatabaseTestsBase
    {
        private readonly DeleteNewRecentUserByUsernameCommandHandler _commandHandler;

        public DeleteNewRecentUserByUsernameCommandTests()
        {
            _commandHandler = new DeleteNewRecentUserByUsernameCommandHandler(DbContext);
        }

        [Test]
        public async Task Should_delete_recent_user()
        {
            const string USERNAME = EmailData.RECENT_USER_USERNAME;

            await Context.Data.SeedRecentUser(USERNAME);

            var command = new DeleteNewRecentUserByUsernameCommand(USERNAME);
            await _commandHandler.Handle(command);

            var recentUser = await Context.Data.GetRecentUserByUsername(USERNAME);
            recentUser.Should().BeNull();
        }

        [Test]
        public void Should_not_delete_non_existent_recent_user()
        {
            const string USERNAME = EmailData.RECENT_USER_USERNAME;

            Assert.ThrowsAsync<RecentUserNotFoundException>(() => _commandHandler.Handle(
                new DeleteNewRecentUserByUsernameCommand(USERNAME)));
        }
    }
}