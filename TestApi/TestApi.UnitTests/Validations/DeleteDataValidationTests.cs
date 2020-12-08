using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Contract.Requests;
using TestApi.Validations;

namespace TestApi.UnitTests.Validations
{
    public class DeleteDataValidationTests
    {
        private DeleteTestHearingDataRequestValidator _validator;
        private const string VALID_TEXT = "Test hearing";
        private const string INVALID_TEXT = "hearing";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new DeleteTestHearingDataRequestValidator();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new DeleteTestHearingDataRequest()
            {
                PartialHearingCaseName = VALID_TEXT
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_if_case_name_empty()
        {
            var request = new DeleteTestHearingDataRequest()
            {
                PartialHearingCaseName = string.Empty
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == DeleteTestHearingDataRequestValidator.EMPTY_REQUEST_ERROR_MESSAGE)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_if_hearing_case_name_does_not_contain_test()
        {
            var request = new DeleteTestHearingDataRequest()
            {
                PartialHearingCaseName = INVALID_TEXT
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == DeleteTestHearingDataRequestValidator.HEARING_CASE_NAME_MUST_CONTAIN_TEST_ERROR_MESSAGE)
                .Should().BeTrue();
        }
    }
}
