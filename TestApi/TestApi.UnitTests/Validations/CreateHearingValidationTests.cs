using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.UnitTests.Controllers;
using TestApi.Validations;

namespace TestApi.UnitTests.Validations
{
    public class CreateHearingValidationTests : ControllerTestsBase
    {
        private CreateHearingRequestValidator _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new CreateHearingRequestValidator();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = CreateHearingRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_empty_users_error()
        {
            var request = CreateHearingRequest();
            request.Users.Clear(); 

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().BeGreaterThan(0);
            result.Errors.Any(x => x.ErrorMessage == CreateHearingRequestValidator.EMPTY_USERS_ERROR_MESSAGE)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_pass_validation_with_winger_in_CACD_hearing()
        {
            var request = CreateHearingRequest();
            request.Users.Add(new UserBuilder(EmailData.FAKE_EMAIL_STEM, 1).AddWinger().BuildUser());
            request.CaseType = HearingData.CACD_CASE_TYPE_NAME;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_with_winger_in_non_CACD_hearing()
        {
            var request = CreateHearingRequest();
            request.Users.Add(new UserBuilder(EmailData.FAKE_EMAIL_STEM, 1).AddWinger().BuildUser());

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().BeGreaterThan(0);
            result.Errors.Any(x => x.ErrorMessage == CreateHearingRequestValidator.WINGERS_CAN_ONLY_BE_IN_CACD_HEARINGS)
                .Should().BeTrue();
        }
    }
}
