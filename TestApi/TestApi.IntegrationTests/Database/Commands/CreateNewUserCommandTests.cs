using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.DAL.Commands;
using TestApi.DAL.Exceptions;
using TestApi.Contract.Enums;

namespace TestApi.IntegrationTests.Database.Commands
{
    public class CreateNewUserCommandTests : DatabaseTestsBase
    {
        private readonly CreateNewUserCommandHandler _commandHandler;

        public CreateNewUserCommandTests()
        {
            _commandHandler = new CreateNewUserCommandHandler(DbContext);
        }

        [Test]
        public async Task Should_create_test_api_user()
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;
            const int NUMBER = 1;

            var request = new UserBuilder(EMAIL_STEM, NUMBER)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .BuildRequest();

            var command = new CreateNewUserCommand
            (
                request.Username, request.ContactEmail, request.FirstName, request.LastName, request.DisplayName, 
                request.Number, request.TestType, request.UserType, request.Application, request.IsProdUser
            );

            await _commandHandler.Handle(command);

            var userId = command.NewUserId;

            var user = await Context.Data.GetUserById(userId);

            user.Application.Should().Be(request.Application);
            user.ContactEmail.Should().Be(request.ContactEmail);
            user.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            user.DisplayName.Should().Be(request.DisplayName);
            user.FirstName.Should().Be(request.FirstName);
            user.LastName.Should().Be(request.LastName);
            user.Number.Should().Be(NUMBER);
            user.Username.Should().Be(request.Username);
            user.UserType.Should().Be(request.UserType);
        }

        [Test]
        public async Task Should_not_create_test_api_user_if_matching_number_exists()
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var firstUser = await Context.Data.SeedUser();

            var request = new UserBuilder(EMAIL_STEM, firstUser.Number)
                .WithUserType(firstUser.UserType)
                .ForApplication(firstUser.Application)
                .BuildRequest();

            var command = new CreateNewUserCommand
            (
                request.Username, request.ContactEmail, request.FirstName, request.LastName, request.DisplayName, 
                request.Number, request.TestType, request.UserType, request.Application, request.IsProdUser
            );

            Assert.ThrowsAsync<MatchingUserWithNumberExistsException>(() => _commandHandler.Handle(command));
        }
    }
}