using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Models;

namespace IAS.Identity.Application.Common.Interface;

public interface IRoleService
{
    Task<PaginatedList<RoleDto>> GetPaginatedListRoles(int pageNumber, int pageSize);

    Task<RoleDto> CreateRole(CreateAndUpdateRole create);

    Task<bool> UpdateRole(CreateAndUpdateRole dto);

    Task<RoleDto> GetRoleById(Guid roleId);

    Task<List<RoleLookUpDto>> GetRoleLookUpDtos();
}