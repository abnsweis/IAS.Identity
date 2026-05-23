using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Domain.Common.Exceptions.Roles;

public class RoleAlreadyAssignedException : DomainException
{
    public RoleAlreadyAssignedException(string username, string roleName)
        : base($"Role  '{roleName}' is already assigned to user '{username}'.")
    {
    }
}