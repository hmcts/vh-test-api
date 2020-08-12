using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries;
using TestApi.Domain.Enums;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetUserByUserTypeApplicationAndNumberQueryTests : DatabaseTestsBase
    {
        private readonly GetUserByUserTypeApplicationAndNumberQueryHandler _query;

        public GetUserByUserTypeApplicationAndNumberQueryTests()
        {
            _query = new GetUserByUserTypeApplicationAndNumberQueryHandler(_dbContext);
        }

        [Test]
        public async Task Should_get_user_by_user_type_application_and_number()
        {
            var user = await _context.TestDataManager.SeedUser();
            var userDetails = await _query.Handle(new GetUserByUserTypeApplicationAndNumberQuery(user.UserType, user.Application, user.Number));
            userDetails.Should().BeEquivalentTo(user);
        }

        [Test]
        public async Task Should_throw_not_found_if_user_with_user_type_does_not_exist()
        {
            var user = await _context.TestDataManager.SeedUser();

            Assert.ThrowsAsync<UserNotFoundException>(() => _query.Handle(
                new GetUserByUserTypeApplicationAndNumberQuery(UserType.None, user.Application, user.Number)));
        }

        [Test]
        public async Task Should_throw_not_found_if_user_with_application_does_not_exist()
        {
            var user = await _context.TestDataManager.SeedUser();

            Assert.ThrowsAsync<UserNotFoundException>(() => _query.Handle(
                new GetUserByUserTypeApplicationAndNumberQuery(user.UserType, Application.None, user.Number)));
        }

        [Test]
        public async Task Should_throw_not_found_if_user_with_number_does_not_exist()
        {
            var user = await _context.TestDataManager.SeedUser();

            Assert.ThrowsAsync<UserNotFoundException>(() => _query.Handle(
                new GetUserByUserTypeApplicationAndNumberQuery(user.UserType, user.Application, -1)));
        }
    }
}
