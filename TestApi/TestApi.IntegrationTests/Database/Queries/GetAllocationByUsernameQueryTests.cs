using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetAllocationByUsernameQueryTests : DatabaseTestsBase
    {
        private readonly GetAllocationByUsernameQueryHandler _query;

        public GetAllocationByUsernameQueryTests()
        {
            _query = new GetAllocationByUsernameQueryHandler(_dbContext);
        }

        [Test]
        public async Task Should_return_allocation_by_username()
        {
            var user = await _context.TestDataManager.SeedUser();
            var allocation = await _context.TestDataManager.SeedAllocation(user.Id);

            var allocationDetails = await _query.Handle(new GetAllocationByUsernameQuery(user.Username));
            allocationDetails.Should().BeEquivalentTo(allocation);
        }

        [Test]
        public async Task Should_not_throw_error_for_nonexistent_username()
        {
            const string USERNAME = "made_up_username@email.com";
            var allocationDetails = await _query.Handle(new GetAllocationByUsernameQuery(USERNAME));
            allocationDetails.Should().BeNull();
        }

        [Test]
        public async Task Should_not_throw_error_for_nonexistent_allocation()
        {
            var user = await _context.TestDataManager.SeedUser();
            var allocationDetails = await _query.Handle(new GetAllocationByUsernameQuery(user.Username));
            allocationDetails.Should().BeNull();
        }
    }
}
