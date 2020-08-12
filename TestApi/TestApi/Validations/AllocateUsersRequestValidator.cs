using FluentValidation;
using TestApi.Contract.Requests;
using TestApi.Domain.Enums;

namespace TestApi.Validations
{
    public class AllocateUsersRequestValidator : AbstractValidator<AllocateUsersRequest>
    {
        public const string EMPTY_USERS_ERROR_MESSAGE = "You must supply more than 1 usertype";
        public const string EMPTY_APPLICATION_ERROR_MESSAGE = "You must supply an application";
        public const string MISSING_CASE_ADMIN_USER_ERROR_MESSAGE = "Users must include case admin";
        public const string MORE_THAN_ONE_JUDGE_ERROR_MESSAGE = "You can only specify 1 judge per allocation";

        public AllocateUsersRequestValidator()
        {
            RuleFor(x => x.UserTypes)
                .NotEmpty().WithMessage(EMPTY_USERS_ERROR_MESSAGE);

            RuleFor(x => x.Application)
                .NotEmpty().WithMessage(EMPTY_APPLICATION_ERROR_MESSAGE);

            RuleFor(x => x.UserTypes)
                .Must(x => x.Contains(UserType.CaseAdmin)).WithMessage(MISSING_CASE_ADMIN_USER_ERROR_MESSAGE);

            RuleFor(x => x.UserTypes)
                .Must(x => x.FindAll(u => u.Equals(UserType.Judge)).Count <= 1).WithMessage(MORE_THAN_ONE_JUDGE_ERROR_MESSAGE);
        }
    }
}
