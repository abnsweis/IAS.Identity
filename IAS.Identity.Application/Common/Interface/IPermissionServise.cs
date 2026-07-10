using IAS.Identity.Application.Common.Dtos.Permissions;
using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Application.Common.Interface;

public interface IPermissionService
{
    Task<PaginatedList<PermissionSummaryDTO>> GetPaginatedListPermissions(int pageNumber, int pageSize);

    Task<PermissionDetailsDTO> GetPermissionById(Guid permissionId);

    Task<PermissionDetailsDTO> CreatePermission(CreatePermissionRequest create);

    Task<bool> UpdatePermission(Guid permissionId, UpdatePermissionRequest update);

    Task<bool> DeletePermissionAsync(Guid permissionId);
}