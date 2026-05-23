using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Application.Common.Dtos.Roles;

public class CreateAndUpdateRole
{
    public Guid RoleId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}