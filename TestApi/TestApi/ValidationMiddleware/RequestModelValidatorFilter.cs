using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using TestApi.Extensions;

namespace TestApi.ValidationMiddleware
{
    public class RequestModelValidatorFilter : IAsyncActionFilter
    {
        private readonly ILogger<RequestModelValidatorFilter> _logger;
        private readonly IRequestModelValidatorService _requestModelValidatorService;

        public RequestModelValidatorFilter(IRequestModelValidatorService requestModelValidatorService,
            ILogger<RequestModelValidatorFilter> logger)
        {
            _requestModelValidatorService = requestModelValidatorService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogDebug("Processing request");
            foreach (var property in context.ActionDescriptor.Parameters)
            {
                var (key, value) = context.ActionArguments.SingleOrDefault(x => x.Key == property.Name);
                if (property.BindingInfo?.BindingSource == BindingSource.Body)
                {
                    var validationFailures = _requestModelValidatorService.Validate(property.ParameterType, value);
                    context.ModelState.AddFluentValidationErrors(validationFailures);
                }

                if (value != null && value.Equals(GetDefaultValue(property.ParameterType)))
                {
                    context.ModelState.AddModelError(key, $"Please provide a valid {key}");
                }
            }

            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList();
                var errorsString = string.Join("; ", errors);
                _logger.LogWarning("Request Validation Failed: {Errors}", errorsString);
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
            else
            {
                await next();
            }
        }

        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}