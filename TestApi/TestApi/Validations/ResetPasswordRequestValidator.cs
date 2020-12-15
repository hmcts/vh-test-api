using System;
using FluentValidation;
using TestApi.Common.Data;
using TestApi.Contract.Requests;

namespace TestApi.Validations
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetUserPasswordRequest>
    {
        private const string AUTOMATION_TEXT = UserData.AUTOMATED_FIRST_NAME_PREFIX;
        public const string EMPTY_USERNAME_ERROR_MESSAGE = "You must supply a username";
        public const string CANNOT_RESET_AUTOMATION_USERS_ERROR_MESSAGE = "Automation Users cannot be reset";

        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Username.Length)
                .GreaterThan(0).WithMessage(EMPTY_USERNAME_ERROR_MESSAGE);

            RuleFor(x => x.Username)
                .Must(x => !x.Contains(AUTOMATION_TEXT, StringComparison.InvariantCultureIgnoreCase))
                .WithMessage(CANNOT_RESET_AUTOMATION_USERS_ERROR_MESSAGE);
        }
    }
}
