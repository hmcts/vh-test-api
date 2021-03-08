using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace TestApi.Middleware.Validation
{
    public class RequestModelValidatorService : IRequestModelValidatorService
    {
        private readonly IValidatorFactory _validatorFactory;

        public RequestModelValidatorService(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public IList<ValidationFailure> Validate(Type requestModel, object modelValue)
        {
            var validator = _validatorFactory.GetValidator(requestModel);
            if (validator == null)
            {
                return new List<ValidationFailure>();
            }
            var context = new ValidationContext<object>(modelValue);
            var result = validator.Validate(context);
            return result.Errors;
        }
    }
}
