using FluentValidation;
using FluentValidation.Results;
using IAS.Identity.Domain.Common.Options;

namespace IAS.Identity.Application.Common.Validators.Password;

public class PasswordFluentValidator : AbstractValidator<string>
{
    public PasswordFluentValidator(PasswordValidator passwordValidator)
    {
        RuleFor(password => password)
            .Custom((password, context) =>
            {
                var result = passwordValidator.Validate(password);
                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        var failure = new ValidationFailure(context.PropertyName, error.Message);
                        failure.ErrorCode = error.Code;   
                        context.AddFailure(failure);
                    }
                }
            });
    }
}