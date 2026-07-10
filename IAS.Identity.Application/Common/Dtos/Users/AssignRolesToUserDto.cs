using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Application.Common.Dtos.Users;

public class AssignRolesToUserRequest
{
    public List<string> Roles { get; set; } = new List<string>();
}