using FluentValidation;
using IAS.Identity.Application.Common.Dtos.Roles;

namespace IAS.Identity.Application.Common.Validators.Roles;

public class RemovePermissionsFromRoleRequestValidator : AbstractValidator<RemovePermissionsFromRoleRequest>
{
    public RemovePermissionsFromRoleRequestValidator()
    {
        RuleForEach(x => x.Permissions)
            .NotEmpty()
                .WithMessage("Permission must not be empty.")
                .WithErrorCode(ErrorCodes.Permissions.PERMISSION_EMPTY)
            .Must(value => Guid.TryParse(value, out var _))
                .WithMessage("Permission ID is not a valid GUID.")
                .WithErrorCode(ErrorCodes.General.INVALID_GUID)
            .When(x => x.Permissions is { Count: > 0 });
    }
}