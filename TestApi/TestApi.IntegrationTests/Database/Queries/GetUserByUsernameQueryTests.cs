﻿using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.DAL.Queries;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetUserByUsernameQueryTests : DatabaseTestsBase
    {
        private readonly GetUserByUsernameQueryHandler _query;

        public GetUserByUsernameQueryTests()
        {
            _query = new GetUserByUsernameQueryHandler(DbContext);
        }

        [Test]
        public async Task Should_get_user_by_username()
        {
            var user = await Context.Data.SeedUser();
            var userDetails = await _query.Handle(new GetUserByUsernameQuery(user.Username));
            userDetails.Should().BeEquivalentTo(user);
        }

        [Test]
        public async Task Should_not_throw_error_if_user_not_found_for_nonexistent_user_id()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;
            var userDetails = await _query.Handle(new GetUserByUsernameQuery(USERNAME));
            userDetails.Should().BeNull();
        }
    }
}