using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Dtos.Users;

namespace IAS.Identity.Application.Common.Dtos.Permissions;

public class PermissionDetailsDTO : PermissionSummaryDTO
{
    public List<RoleDto> RoleNames { get; set; } = new();
    public List<UserSummaryDTO> UserNames { get; set; } = new();
}
