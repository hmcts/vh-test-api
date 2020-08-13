using FluentValidation;
using TestApi.Contract.Requests;

namespace TestApi.Validations
{
    public class UnallocateUsersRequestValidator : AbstractValidator<UnallocateUsersRequest>
    {
        public const string EMPTY_USERNAMES_ERROR_MESSAGE = "You must supply more than 1 username";

        public UnallocateUsersRequestValidator()
        {
            RuleFor(x => x.Usernames)
                .NotEmpty().WithMessage(EMPTY_USERNAMES_ERROR_MESSAGE);
        }
    }
}