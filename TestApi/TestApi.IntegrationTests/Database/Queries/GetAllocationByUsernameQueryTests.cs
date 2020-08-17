using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Queries;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetAllocationByUsernameQueryTests : DatabaseTestsBase
    {
        private readonly GetAllocationByUsernameQueryHandler _query;

        public GetAllocationByUsernameQueryTests()
        {
            _query = new GetAllocationByUsernameQueryHandler(DbContext);
        }

        [Test]
        public async Task Should_return_allocation_by_username()
        {
            var user = await Context.Data.SeedUser();
            var allocation = await Context.Data.SeedAllocation(user.Id);

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
            var user = await Context.Data.SeedUser();
            var allocationDetails = await _query.Handle(new GetAllocationByUsernameQuery(user.Username));
            allocationDetails.Should().BeNull();
        }
    }
}