using FluentValidation;
using TestApi.Contract.Requests;

namespace TestApi.Validations
{
    public class DeleteTestHearingDataRequestValidator : AbstractValidator<DeleteTestHearingDataRequest>
    {
        private const string REQUIRED_TEXT = "TEST";
        public const string EMPTY_REQUEST_ERROR_MESSAGE = "You must supply one partial case name or number text";
        public const string HEARING_CASE_NAME_MUST_CONTAIN_TEST_ERROR_MESSAGE =
            "The supplied hearing case name must contain the name 'test'";

        public DeleteTestHearingDataRequestValidator()
        {
            RuleFor(x => x.PartialHearingCaseName.Length)
                .GreaterThan(0).WithMessage(EMPTY_REQUEST_ERROR_MESSAGE);

            RuleFor(x => x.PartialHearingCaseName)
                .Must(x => x.Length == 0 || x.ToUpperInvariant().Contains(REQUIRED_TEXT))
                .WithMessage(HEARING_CASE_NAME_MUST_CONTAIN_TEST_ERROR_MESSAGE);
        }
    }
}
