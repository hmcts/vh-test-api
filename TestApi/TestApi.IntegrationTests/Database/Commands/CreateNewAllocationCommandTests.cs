﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Commands;
using TestApi.DAL.Exceptions;

namespace TestApi.IntegrationTests.Database.Commands
{
    public class CreateNewAllocationCommandTests : DatabaseTestsBase
    {
        private readonly CreateNewAllocationCommandHandler _commandHandler;

        public CreateNewAllocationCommandTests()
        {
            _commandHandler = new CreateNewAllocationCommandHandler(DbContext);
        }

        [Test]
        public async Task Should_create_new_allocation()
        {
            var user = await Context.Data.SeedUser();

            var command = new CreateNewAllocationByUserIdCommand(user.Id);
            await _commandHandler.Handle(command);

            command.NewAllocationId.Should().NotBeEmpty();

            var allocation = await Context.Data.GetAllocationByUserId(user.Id);
            allocation.Allocated.Should().BeFalse();
            allocation.ExpiresAt.Should().BeNull();
            allocation.UserId.Should().Be(user.Id);
            allocation.Username.Should().Be(user.Username);
        }

        [Test]
        public void Should_not_create_allocation_without_user()
        {
            Assert.ThrowsAsync<UserNotFoundException>(() => _commandHandler.Handle(
                new CreateNewAllocationByUserIdCommand(Guid.NewGuid())));
        }

        [Test]
        public async Task Should_not_create_allocation_if_exists()
        {
            var user = await Context.Data.SeedUser();
            await Context.Data.SeedAllocation(user.Id);

            Assert.ThrowsAsync<AllocationAlreadyExistsException>(() => _commandHandler.Handle(
                new CreateNewAllocationByUserIdCommand(user.Id)));
        }
    }
}