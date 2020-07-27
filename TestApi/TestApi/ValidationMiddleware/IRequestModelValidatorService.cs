using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace TestApi.ValidationMiddleware
{
    public interface IRequestModelValidatorService
    {
        IList<ValidationFailure> Validate(Type requestModel, object modelValue);
    }
}