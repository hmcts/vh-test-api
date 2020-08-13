using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Commands;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries;

namespace TestApi.IntegrationTests.Database.Commands
{
    public class AllocateByUserIdCommandTests : DatabaseTestsBase
    {
        private readonly AllocateByUserIdCommandHandler _commandHandler;
        private readonly GetAllocationByUserIdQueryHandler _query;

        public AllocateByUserIdCommandTests()
        {
            _commandHandler = new AllocateByUserIdCommandHandler(_dbContext);
            _query = new GetAllocationByUserIdQueryHandler(_dbContext);
        }

        [Test]
        public async Task Should_allocate_by_user_id()
        {
            var user = await _context.TestDataManager.SeedUser();
            await _context.TestDataManager.SeedAllocation(user.Id);
            const int MINUTES = 2;

            var command = new AllocateByUserIdCommand(user.Id, MINUTES);
            await _commandHandler.Handle(command);

            command.User.Should().NotBeNull();

            var allocationDetails = await _query.Handle(new GetAllocationByUserIdQuery(user.Id));

            allocationDetails.Allocated.Should().BeTrue();
            allocationDetails.ExpiresAt.Should()
                .BeCloseTo(DateTime.UtcNow.AddMinutes(MINUTES), TimeSpan.FromSeconds(5));
        }

        [Test]
        public void Should_throw_error_if_user_does_not_exist()
        {
            Assert.ThrowsAsync<UserNotFoundException>(() => _commandHandler.Handle(
                new AllocateByUserIdCommand(Guid.NewGuid())));
        }

        [Test]
        public async Task Should_throw_error_if_allocation_does_not_exist()
        {
            var user = await _context.TestDataManager.SeedUser();

            Assert.ThrowsAsync<UserAllocationNotFoundException>(() => _commandHandler.Handle(
                new AllocateByUserIdCommand(user.Id)));
        }

        [Test]
        public async Task Should_throw_error_if_user_already_allocated()
        {
            var user = await _context.TestDataManager.SeedUser();
            await _context.TestDataManager.SeedAllocation(user.Id);
            await _context.TestDataManager.AllocateUser(user.Id);

            Assert.ThrowsAsync<UserUnavailableException>(() => _commandHandler.Handle(
                new AllocateByUserIdCommand(user.Id)));
        }
    }
}