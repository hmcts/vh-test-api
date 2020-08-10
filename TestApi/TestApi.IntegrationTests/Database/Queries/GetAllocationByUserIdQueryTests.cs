using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetAllocationByUserIdQueryTests : DatabaseTestsBase
    {
        private readonly GetAllocationByUserIdQueryHandler _query;

        public GetAllocationByUserIdQueryTests()
        {
            _query = new GetAllocationByUserIdQueryHandler(_dbContext);
        }

        [Test]
        public async Task Should_get_allocation_details_by_user_id()
        {
            var user = await _context.TestDataManager.SeedUser();
            var allocation = await _context.TestDataManager.SeedAllocation(user.Id);

            var allocationDetails = await _query.Handle(new GetAllocationByUserIdQuery(user.Id));

            allocationDetails.UserId.Should().Be(user.Id);
            allocationDetails.Username.Should().Be(user.Username);
            allocationDetails.Allocated.Should().Be(allocation.Allocated);
            allocationDetails.ExpiresAt.Should().Be(allocation.ExpiresAt);
        }

        [Test]
        public void Should_throw_error_if_user_does_not_exist()
        {
            Assert.ThrowsAsync<UserNotFoundException>(() => _query.Handle(
                new GetAllocationByUserIdQuery(Guid.NewGuid())));
        }

        [Test]
        public async Task Should_throw_error_if_allocation_does_not_exist()
        {
            var user = await _context.TestDataManager.SeedUser();

            Assert.ThrowsAsync<UserAllocationNotFoundException>(() => _query.Handle(
                new GetAllocationByUserIdQuery(user.Id)));
        }
    }
}
