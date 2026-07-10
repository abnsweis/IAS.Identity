using FluentValidation;
using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Application.Common.Validators.Roles;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleDTO>
{
    public UpdateRoleValidator()
    {
        Include(new CreateRoleValidator());
    }
}