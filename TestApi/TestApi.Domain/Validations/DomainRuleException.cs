using System;

namespace TestApi.Domain.Validations
{
    public class DomainRuleException : Exception
    {
        public DomainRuleException(ValidationFailures validationFailures)
        {
            ValidationFailures = validationFailures;
        }

        public DomainRuleException(string name, string message)
        {
            ValidationFailures.Add(new ValidationFailure(name, message));
        }

        public ValidationFailures ValidationFailures { get; } = new ValidationFailures();
    }
}