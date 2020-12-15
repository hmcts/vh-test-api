using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Validations;

namespace TestApi.UnitTests.Validations
{
    public class ResetPasswordValidationTests
    {
        private ResetPasswordRequestValidator _validator;
        private const string VALID_TEXT = "test_username";
        private const string AUTOMATION_USER = UserData.AUTOMATED_FIRST_NAME_PREFIX;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new ResetPasswordRequestValidator();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new ResetUserPasswordRequest()
            {
                Username = VALID_TEXT
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_if_username_empty()
        {
            var request = new ResetUserPasswordRequest()
            {
                Username = string.Empty
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ResetPasswordRequestValidator.EMPTY_USERNAME_ERROR_MESSAGE)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_if_username_contains_automation()
        {
            var request = new ResetUserPasswordRequest()
            {
                Username = AUTOMATION_USER
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ResetPasswordRequestValidator.CANNOT_RESET_AUTOMATION_USERS_ERROR_MESSAGE)
                .Should().BeTrue();
        }
    }
}
