using FluentValidation;
using IAS.Identity.Application.Common.Dtos.Permissions;

namespace IAS.Identity.Application.Common.Validators.Permissions;

public class UpdatePermissionValidator : AbstractValidator<UpdatePermissionRequest>
{
    public UpdatePermissionValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty()
               .WithMessage("Permission name is required.")
               .WithErrorCode(ErrorCodes.Permissions.PERMISSION_NAME_REQUIRED)
           .MaximumLength(100)
               .WithMessage("Permission name must not exceed 100 characters.")
               .WithErrorCode(ErrorCodes.Permissions.PERMISSION_NAME_TOO_LONG);
        RuleFor(x => x.Description)
            .MaximumLength(250)
            .WithMessage("Description must not exceed 250 characters.")
            .WithErrorCode(ErrorCodes.Permissions.PERMISSION_DESCRIPTION_TOO_LONG);
    }
}