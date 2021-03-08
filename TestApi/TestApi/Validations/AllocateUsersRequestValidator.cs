using System;
using FluentValidation;
using TestApi.Contract.Requests;
using TestApi.Contract.Enums;

namespace TestApi.Validations
{
    public class AllocateUsersRequestValidator : AbstractValidator<AllocateUsersRequest>
    {
        public const string EMPTY_USERS_ERROR_MESSAGE = "You must supply more than 1 usertype";
        public const string EMPTY_APPLICATION_ERROR_MESSAGE = "You must supply an application";
        public const string MORE_THAN_ONE_JUDGE_ERROR_MESSAGE = "You can only specify 1 judge per allocation";
        public const string EXPIRES_IN_GREATER_THAN_ZERO_ERROR_MESSAGE = "Expires in must be greater than 0";
        public const string EXPIRES_IN_LESS_THAN_THIRTY_DAYS_ERROR_MESSAGE = "Expires in must be less than or equal to 30 days";

        public AllocateUsersRequestValidator()
        {
            RuleFor(x => x.UserTypes)
                .NotEmpty().WithMessage(EMPTY_USERS_ERROR_MESSAGE);

            RuleFor(x => x.Application)
                .NotEmpty().WithMessage(EMPTY_APPLICATION_ERROR_MESSAGE);

            RuleFor(x => x.UserTypes)
                .Must(x => x.FindAll(u => u.Equals(UserType.Judge)).Count <= 1)
                .WithMessage(MORE_THAN_ONE_JUDGE_ERROR_MESSAGE);

            RuleFor(x => x.ExpiryInMinutes)
                .GreaterThan(0).WithMessage(EXPIRES_IN_GREATER_THAN_ZERO_ERROR_MESSAGE);

            RuleFor(x => x.ExpiryInMinutes)
                .LessThanOrEqualTo(Convert.ToInt32(TimeSpan.FromDays(30).TotalMinutes)).WithMessage(EXPIRES_IN_LESS_THAN_THIRTY_DAYS_ERROR_MESSAGE);
        }
    }
}