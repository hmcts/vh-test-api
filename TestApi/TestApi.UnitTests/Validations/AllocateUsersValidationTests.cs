using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Domain.Enums;
using TestApi.Validations;

namespace TestApi.UnitTests.Validations
{
    public class AllocateUsersValidationTests
    {
        private AllocateUsersRequestValidator _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AllocateUsersRequestValidator();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithDefaultTypes()
                .ForApplication(Application.TestApi)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_empty_users_error()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithEmptyUsers()
                .ForApplication(Application.TestApi)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().BeGreaterThan(0);
            result.Errors.Any(x => x.ErrorMessage == AllocateUsersRequestValidator.EMPTY_USERS_ERROR_MESSAGE)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_empty_application_error()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithDefaultTypes()
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUsersRequestValidator.EMPTY_APPLICATION_ERROR_MESSAGE);
        }

        [Test]
        public async Task Should_return_missing_case_admin_error()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithoutCaseAdmin()
                .ForApplication(Application.TestApi)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUsersRequestValidator.MISSING_CASE_ADMIN_USER_ERROR_MESSAGE);
        }

        [Test]
        public async Task Should_return_more_than_one_judge_error()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithMoreThanOneJudge()
                .ForApplication(Application.TestApi)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUsersRequestValidator.MORE_THAN_ONE_JUDGE_ERROR_MESSAGE);
        }
    }
}
