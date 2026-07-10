using IAS.Identity.Application.Common.Dtos.Permissions;
using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Models;

namespace IAS.Identity.Application.Common.Interface;

public interface IRoleService
{
    Task<PaginatedList<RoleDto>> GetPaginatedListRoles(int pageNumber, int pageSize);

    Task<RoleDto> CreateRole(CreateRoleDTO create);

    Task<bool> UpdateRole(Guid roleId, UpdateRoleDTO dto);

    Task<RoleDto> GetRoleById(Guid roleId);

    Task<List<RoleLookUpDto>> GetRoleLookUpDtos();

    Task DeleteRole(Guid id);

    Task AssignPermissionsToRole(Guid roleId, AssignPermissionsToRoleRequest request);

    Task RemovePermussionsFromRole(Guid roleId, RemovePermissionsFromRoleRequest request);

    Task<PaginatedList<PermissionSummaryDTO>> GetPaginatedListPermissionsByRoleId(Guid roleId, int pageNumber, int pageSize);
}