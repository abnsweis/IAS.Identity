using FluentValidation;
using IAS.Identity.Application.Common;
using IAS.Identity.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IAS.Identity.API.Infrastructure.Filters
{
    public class ValidationFilter<T> : IAsyncActionFilter where T : class
    {
        private readonly IValidator<T> _validator;

        public ValidationFilter(IValidator<T> validator)
        {
            this._validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var model = context.ActionArguments.Values.OfType<T>().FirstOrDefault();

            if (model is null)
                throw new BadRequestException("Request body is required.", ErrorCodes.General.REQUEST_BODY_REQUIRED);

            var validationResult = _validator.Validate(model);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new { errorCode = e.ErrorCode, message = e.ErrorMessage })

                    );

                context.Result = new BadRequestObjectResult(new
                {
                    status = 400,
                    title = "One or more validation errors occurred.",
                    errors
                });
                return;
            }

            await next();
        }
    }
}