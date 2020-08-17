using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Commands;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries;

namespace TestApi.IntegrationTests.Database.Commands
{
    public class UnallocateUserCommandTests : DatabaseTestsBase
    {
        private readonly UnallocateUserCommandHandler _commandHandler;
        private readonly GetAllocationByUserIdQueryHandler _query;

        public UnallocateUserCommandTests()
        {
            _commandHandler = new UnallocateUserCommandHandler(DbContext);
            _query = new GetAllocationByUserIdQueryHandler(DbContext);
        }

        [Test]
        public async Task Should_unallocate_by_user_id()
        {
            var user = await Context.Data.SeedUser();
            await Context.Data.SeedAllocation(user.Id);
            const int MINUTES = 1;
            await Context.Data.AllocateUser(user.Id, MINUTES);

            var allocationDetails = await _query.Handle(new GetAllocationByUserIdQuery(user.Id));
            allocationDetails.Allocated.Should().BeTrue();
            allocationDetails.ExpiresAt.Should()
                .BeCloseTo(DateTime.UtcNow.AddMinutes(MINUTES), TimeSpan.FromSeconds(5));

            var command = new UnallocateByUsernameCommand(user.Username);
            await _commandHandler.Handle(command);

            allocationDetails = await _query.Handle(new GetAllocationByUserIdQuery(user.Id));
            allocationDetails.Allocated.Should().BeFalse();
            allocationDetails.ExpiresAt.Should().BeNull();
        }

        [Test]
        public void Should_throw_error_if_username_does_not_exist()
        {
            const string USERNAME = "made_up_username@email.com";

            Assert.ThrowsAsync<UserNotFoundException>(() => _commandHandler.Handle(
                new UnallocateByUsernameCommand(USERNAME)));
        }

        [Test]
        public async Task Should_throw_error_if_user_allocation_does_not_exist()
        {
            var user = await Context.Data.SeedUser();

            Assert.ThrowsAsync<UserAllocationNotFoundException>(() => _commandHandler.Handle(
                new UnallocateByUsernameCommand(user.Username)));
        }
    }
}
