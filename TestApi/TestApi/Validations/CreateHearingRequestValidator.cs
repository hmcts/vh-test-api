using System.Linq;
using FluentValidation;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Domain.Enums;

namespace TestApi.Validations
{
    public class CreateHearingRequestValidator : AbstractValidator<CreateHearingRequest>
    {
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
        }
    }
}
