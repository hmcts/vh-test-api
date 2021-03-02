using System;
using FluentValidation;

namespace TestApi.Middleware.Validation
{
    public class RequestModelValidatorFactory : ValidatorFactoryBase
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestModelValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            var validator = (IValidator)_serviceProvider.GetService(validatorType);
            var isApiRequestContract = validatorType.FullName?.Contains("NotificationApi.Contract.Requests");
            if (validator == null && isApiRequestContract.HasValue && isApiRequestContract.Value)
            {
                throw new InvalidOperationException($"No validator found for {validatorType}");
            }
            return validator;
        }
    }
}
