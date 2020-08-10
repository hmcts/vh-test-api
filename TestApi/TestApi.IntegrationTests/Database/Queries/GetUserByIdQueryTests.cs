using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetUserByIdQueryTests : DatabaseTestsBase
    {
        private readonly GetUserByIdQueryHandler _query;

        public GetUserByIdQueryTests()
        {
            _query = new GetUserByIdQueryHandler(_dbContext);
        }

        [Test]
        public async Task Should_get_user_by_id()
        {
            var user = await _context.TestDataManager.SeedUser();
            var userDetails = await _query.Handle(new GetUserByIdQuery(user.Id));
            userDetails.Application.Should().Be(user.Application);
            userDetails.ContactEmail.Should().Be(user.ContactEmail);
            userDetails.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            userDetails.DisplayName.Should().Be(user.DisplayName);
            userDetails.FirstName.Should().Be(user.FirstName);
            userDetails.LastName.Should().Be(user.LastName);
            userDetails.Number.Should().Be(user.Number);
            userDetails.Username.Should().Be(user.Username);
            userDetails.UserType.Should().Be(user.UserType);
        }

        [Test]
        public void Should_throw_user_not_found_for_nonexistent_user_id()
        {
            Assert.ThrowsAsync<UserNotFoundException>(() => _query.Handle(
                new GetUserByIdQuery(Guid.NewGuid())));
        }
    }
}
