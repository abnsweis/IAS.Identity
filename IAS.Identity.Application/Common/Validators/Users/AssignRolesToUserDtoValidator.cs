using FluentValidation;
using IAS.Identity.Application.Common.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Application.Common.Validators.Users;

public class AssignRolesToUserDtoValidator : AbstractValidator<AssignRolesToUserRequest>
{
    public AssignRolesToUserDtoValidator()
    {
        RuleForEach(x => x.Roles)
          .NotEmpty()
              .WithMessage("Role ID must not be empty.")
              .WithErrorCode(ErrorCodes.Roles.ROLE_EMPTY_GUID)
          .Must(value => Guid.TryParse(value, out _))
              .WithMessage("Role ID is not a valid GUID.")
              .WithErrorCode(ErrorCodes.Roles.ROLE_INVALID_GUID)
          .When(x => x.Roles is { Count: > 0 });
    }
}