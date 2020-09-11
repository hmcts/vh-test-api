using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.DAL.Commands;
using TestApi.DAL.Exceptions;

namespace TestApi.IntegrationTests.Database.Commands
{
    public class CreateNewRecentUserByUsernameCommandTests : DatabaseTestsBase
    {
        private readonly CreateNewRecentUserByUsernameCommandHandler _commandHandler;

        public CreateNewRecentUserByUsernameCommandTests()
        {
            _commandHandler = new CreateNewRecentUserByUsernameCommandHandler(DbContext);
        }

        [Test]
        public async Task Should_create_new_recent_user()
        {
            const string USERNAME = EmailData.RECENT_USER_USERNAME;

            var command = new CreateNewRecentUserByUsernameCommand(USERNAME);
            await _commandHandler.Handle(command);

            var recentUser = await Context.Data.GetRecentUserByUsername(USERNAME);
            recentUser.IsRecentlyCreated().Should().BeTrue();
            recentUser.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Test]
        public async Task Should_not_create_recent_user_if_exists()
        {
            const string USERNAME = EmailData.RECENT_USER_USERNAME;

            await Context.Data.SeedRecentUser(USERNAME);

            Assert.ThrowsAsync<RecentUserAlreadyExistsException>(() => _commandHandler.Handle(
                new CreateNewRecentUserByUsernameCommand(USERNAME)));
        }
    }
}