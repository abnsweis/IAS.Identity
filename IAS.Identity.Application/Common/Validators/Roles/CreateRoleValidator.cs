using FluentValidation;
using IAS.Identity.Application.Common.Dtos.Roles;

namespace IAS.Identity.Application.Common.Validators.Roles;

public class CreateRoleValidator : AbstractValidator<CreateRoleDTO>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Role name is required.")
                .WithErrorCode(ErrorCodes.Roles.ROLE_NAME_REQUIRED)
            .MaximumLength(100)
                .WithMessage("Role name must not exceed 100 characters.")
                .WithErrorCode(ErrorCodes.Roles.ROLE_NAME_TOO_LONG);
        RuleFor(x => x.Description)
            .MaximumLength(250)
            .WithMessage("Description must not exceed 250 characters.")
            .WithErrorCode(ErrorCodes.Roles.ROLE_DESCRIPTION_TOO_LONG);
    }
}