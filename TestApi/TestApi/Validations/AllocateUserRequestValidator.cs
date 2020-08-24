using System;
using FluentValidation;
using TestApi.Contract.Requests;

namespace TestApi.Validations
{
    public class AllocateUserRequestValidator : AbstractValidator<AllocateUserRequest>
    {
        public const string EMPTY_USER_ERROR_MESSAGE = "You must supply a usertype";
        public const string EXPIRES_IN_GREATER_THAN_ZERO_ERROR_MESSAGE = "Expires in must be greater than 0";
        public const string EXPIRES_IN_LESS_THAN_TWELVE_HOURS_ERROR_MESSAGE = "Expires in must be less than or equal to 12 hours";

        public AllocateUserRequestValidator()
        {
            RuleFor(x => x.UserType)
                .NotNull().WithMessage(EMPTY_USER_ERROR_MESSAGE);

            RuleFor(x => x.ExpiryInMinutes)
                .GreaterThan(0).WithMessage(EXPIRES_IN_GREATER_THAN_ZERO_ERROR_MESSAGE);

            RuleFor(x => x.ExpiryInMinutes)
                .LessThanOrEqualTo(Convert.ToInt32(TimeSpan.FromHours(12).TotalMinutes)).WithMessage(EXPIRES_IN_LESS_THAN_TWELVE_HOURS_ERROR_MESSAGE);
        }
    }
}