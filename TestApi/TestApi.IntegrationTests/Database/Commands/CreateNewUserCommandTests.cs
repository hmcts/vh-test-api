using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.DAL.Commands;
using TestApi.DAL.Exceptions;
using TestApi.Domain.Enums;

namespace TestApi.IntegrationTests.Database.Commands
{
    public class CreateNewUserCommandTests : DatabaseTestsBase
    {
        private readonly CreateNewUserCommandHandler _commandHandler;

        public CreateNewUserCommandTests()
        {
            _commandHandler = new CreateNewUserCommandHandler(_dbContext);
        }

        [Test]
        public async Task Should_create_test_api_user()
        {
            const string EMAIL_STEM = "made_up_email_stem_for_test";
            const int NUMBER = 1;

            var request = new UserBuilder(EMAIL_STEM, NUMBER)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .BuildRequest();

            var command = new CreateNewUserCommand
            (
                request.Username, request.ContactEmail, request.FirstName, request.LastName,
                request.DisplayName, request.Number, request.UserType, request.Application
            );

            await _commandHandler.Handle(command);

            var userId = command.NewUserId;

            var user = await _context.TestDataManager.GetUserById(userId);

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
            const string EMAIL_STEM = "made_up_email_stem_for_test";

            var firstUser = await _context.TestDataManager.SeedUser();

            var request = new UserBuilder(EMAIL_STEM, firstUser.Number)
                .WithUserType(firstUser.UserType)
                .ForApplication(firstUser.Application)
                .BuildRequest();

            var command = new CreateNewUserCommand
            (
                request.Username, request.ContactEmail, request.FirstName, request.LastName,
                request.DisplayName, request.Number, request.UserType, request.Application
            );

            Assert.ThrowsAsync<MatchingUserWithNumberExistsException>(() => _commandHandler.Handle(command));
        }
    }
}