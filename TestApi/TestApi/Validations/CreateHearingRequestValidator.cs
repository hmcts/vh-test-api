using System.Linq;
using FluentValidation;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Contract.Enums;

namespace TestApi.Validations
{
    public class CreateHearingRequestValidator : AbstractValidator<CreateHearingRequest>
    {
        public const string INTERPRETERS_COUNT_SHOULD_BE_LESS_THAN_INDIVIDUALS_COUNT_ERROR_MESSAGE = "Interpreters count should be less than the number of indivudals";
        public const string EMPTY_USERS_ERROR_MESSAGE = "You must supply a list of users";
        public const string WINGERS_CAN_ONLY_BE_IN_CACD_HEARINGS = "Wingers can only join CACD hearings";

        public CreateHearingRequestValidator()
        {
            RuleFor(x => x.Users.Count)
                .GreaterThan(0).WithMessage(EMPTY_USERS_ERROR_MESSAGE);

            When(x => x.Users.Any(u => u.UserType == UserType.Winger), () =>
            {
                RuleFor(x => x.CaseType)
                    .Must(x => x.Equals(HearingData.CACD_CASE_TYPE_NAME))
                    .WithMessage(WINGERS_CAN_ONLY_BE_IN_CACD_HEARINGS);
            });

            When(x => x.Users.Any(u => u.UserType == UserType.Interpreter), () =>
            {
                RuleFor(x => x.Users.Count(x => x.UserType == UserType.Interpreter))
                    .LessThanOrEqualTo(y => y.Users.Count(z => z.UserType == UserType.Individual))
                    .WithMessage(INTERPRETERS_COUNT_SHOULD_BE_LESS_THAN_INDIVIDUALS_COUNT_ERROR_MESSAGE);
            });
        }
    }
}
