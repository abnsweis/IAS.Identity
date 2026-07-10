using FluentValidation;
using IAS.Identity.Application.Common.Dtos.Users;

namespace IAS.Identity.Application.Common.Validators.Users;

public class ChangeUserStatusValidator : AbstractValidator<ChangeUserStatusDto>
{
    public ChangeUserStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid status value.")
            .WithErrorCode("ENUM_ERROR");
    }
}