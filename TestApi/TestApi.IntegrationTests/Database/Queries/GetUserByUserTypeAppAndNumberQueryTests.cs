using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.DAL.Queries;
using TestApi.Contract.Enums;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetUserByUserTypeAppAndNumberQueryTests : DatabaseTestsBase
    {
        private readonly GetUserByUserTypeAppAndNumberQueryHandler _query;

        public GetUserByUserTypeAppAndNumberQueryTests()
        {
            _query = new GetUserByUserTypeAppAndNumberQueryHandler(DbContext);
        }

        [Test]
        public async Task Should_get_user_by_user_type_application_and_number()
        {
            var user = await Context.Data.SeedUser();
            var userDetails =
                await _query.Handle(
                    new GetUserByUserTypeAppAndNumberQuery(user.UserType, user.Application, user.Number, user.IsProdUser));
            userDetails.Should().BeEquivalentTo(user);
        }

        [Test]
        public async Task Should_not_throw_error_if_user_with_user_type_does_not_exist()
        {
            var user = await Context.Data.SeedUser();
            var userDetails =
                await _query.Handle(
                    new GetUserByUserTypeAppAndNumberQuery(UserType.None, user.Application, user.Number, user.IsProdUser));
            userDetails.Should().BeNull();
        }

        [Test]
        public async Task Should_not_throw_error_if_user_with_application_does_not_exist()
        {
            var user = await Context.Data.SeedUser();

            var userDetails =
                await _query.Handle(
                    new GetUserByUserTypeAppAndNumberQuery(user.UserType, Application.None, user.Number, user.IsProdUser));
            userDetails.Should().BeNull();
        }

        [Test]
        public async Task Should_not_throw_error_if_user_with_number_does_not_exist()
        {
            var user = await Context.Data.SeedUser();

            var userDetails =
                await _query.Handle(new GetUserByUserTypeAppAndNumberQuery(user.UserType, user.Application, -1, user.IsProdUser));
            userDetails.Should().BeNull();
        }
    }
}