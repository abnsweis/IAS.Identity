using IAS.Identity.Application.Common;
using IAS.Identity.Application.Common.Dtos.Permissions;
using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Exceptions;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Application.Common.Models;
using IAS.Identity.Domain.Entities;
using IAS.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAS.Identity.Infrastructure.Services;

internal class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
        this._context = context;
    }

    public async Task AssignPermissionsToRole(Guid roleId, AssignPermissionsToRoleRequest request)
    {
        // get the role with its current permissions
        var role = await _context.Roles.Where(r => !r.IsDeleted).Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == roleId);

        // If the role does not exist, throw a NotFoundException
        if (role == null)
            throw new NotFoundException($"Role with id '{roleId}' was not found.", ErrorCodes.Roles.ROLE_NOT_FOUND);

        // If no permissions are provided in the request, throw a BadRequestException
        if (!request.Permissions.Any())
            throw new BadRequestException("At least one permission must be assigned to the role.", ErrorCodes.Permissions.PERMISSIONS_REQUIRED);

        var permissionsIds = new List<Guid>();
        foreach (var p in request.Permissions)
        {
            if (!Guid.TryParse(p, out var guid))
                throw new BadRequestException("One or more permissions do not exist.", ErrorCodes.General.INVALID_GUID);
            permissionsIds.Add(guid);
        }

        var validIds = await _context.Permissions.Where(p => permissionsIds.Contains(p.Id)).Select(p => p.Id).ToListAsync();

        if (validIds.Count != request.Permissions.Count)
            throw new BadRequestException("One or more permissions are invalid.", ErrorCodes.General.INVALID_GUID);

        // Get the existing permission IDs for the role to avoid duplicates
        var existingIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();

        var newPermissions = validIds.Where(p => !existingIds.Contains(p)).Select(p => new RolePermission
        {
            PermissionId = p,
            RoleId = roleId
        }).ToList();

        await _context.RolePermissions.AddRangeAsync(newPermissions);
        await _context.SaveChangesAsync();
    }

    public async Task<RoleDto> CreateRole(CreateRoleDTO create)
    {
        if (string.IsNullOrEmpty(create.Name))
        {
            throw new BadRequestException("role name required", ErrorCodes.Roles.ROLE_NAME_REQUIRED);
        }

        var newRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = create.Name,
            Description = create.Description
        };

        await _context.Roles.AddAsync(newRole);
        await _context.SaveChangesAsync();

        return _MapToRoleDto(newRole);
    }

    public async Task DeleteRole(Guid id)
    {
        var role = await _context.Roles.Where(r => !r.IsDeleted).FirstOrDefaultAsync(r => r.Id == id);

        // Check if the role exists
        if (role == null)
            throw new NotFoundException(ErrorCodes.Roles.ROLE_NOT_FOUND, $"Role with id '{id}' was not found.");
        // Check if the role is assigned to any users
        if (await _context.UserRoles.AnyAsync(ur => ur.RoleId == role.Id))
            throw new ConflictException($"Role with id '{id}' cannot be deleted because it is assigned to one or more users.", ErrorCodes.Roles.ROLE_ASSIGNED_TO_USERS);

        // Soft delete the role
        role.Delete();
        // Save changes to the database
        await _context.SaveChangesAsync();
    }

    public async Task<PaginatedList<PermissionSummaryDTO>> GetPaginatedListPermissionsByRoleId(Guid roleId, int pageNumber, int pageSize)
    {
        var permissions = _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Include(rp => rp.Role)
            .Include(rp => rp.Role)
                .ThenInclude(r => r.UserRoles)
            .Include(rp => rp.Role)
                .ThenInclude(r => r.RolePermissions)
            .Select(rp => new PermissionSummaryDTO
            {
                Id = rp.PermissionId,
                Name = rp.Permission.Name,
                Description = rp.Permission.Description,
                RolesCount = rp.Permission.RolePermissions.Count,
                UsersCount = rp.Permission.RolePermissions.SelectMany(rp => rp.Role.UserRoles).Count()
            });

        var paginatedPermissions = await PaginatedList<PermissionSummaryDTO>.CreateAsync(permissions.AsQueryable(), pageNumber, pageSize);

        return paginatedPermissions;
    }

    public Task<PaginatedList<RoleDto>> GetPaginatedListRoles(int pageNumber, int pageSize)
    {
        var roleDtos = _context.Roles
            .Include(r => r.UserRoles)
            .Include(r => r.RolePermissions)
            .Where(r => !r.IsDeleted)
            .Select(role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                PermissionsCount = role.RolePermissions.Count,
                UsersCount = role.UserRoles.Count
            })
            .AsQueryable();

        var paginatedRoles = PaginatedList<RoleDto>.CreateAsync(roleDtos, pageNumber, pageSize);

        return paginatedRoles;
    }

    public async Task<RoleDto> GetRoleById(Guid roleId)
    {
        var role = await _context.Roles.Where(r => !r.IsDeleted).Include(r => r.UserRoles).Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == roleId);
        if (role == null)
            throw new NotFoundException(ErrorCodes.Roles.ROLE_NOT_FOUND, $"Role with id '{roleId}' was not found.");
        return _MapToRoleDto(role);
    }

    public Task<List<RoleLookUpDto>> GetRoleLookUpDtos()
    {
        var roleLookUpDtos = _context.Roles
            .Where(r => !r.IsDeleted).Select(r => new RoleLookUpDto
            {
                RoleId = r.Id,
                Name = r.Name
            }).ToListAsync();
        return roleLookUpDtos;
    }

    public Task RemovePermussionsFromRole(Guid roleId, RemovePermissionsFromRoleRequest request)
    {
        var role = _context.Roles.Where(r => !r.IsDeleted).Include(r => r.RolePermissions).FirstOrDefault(r => r.Id == roleId);

        if (role == null)
            throw new NotFoundException(ErrorCodes.Roles.ROLE_NOT_FOUND, $"Role with id '{roleId}' was not found.");

        var permissionsIds = new List<Guid>();
        foreach (var p in request.Permissions)
        {
            if (!Guid.TryParse(p, out var guid))
                throw new BadRequestException("One or more permissions do not exist.", ErrorCodes.General.INVALID_GUID);
            permissionsIds.Add(guid);
        }

        var permissionsToRemove = role.RolePermissions.Where(rp => permissionsIds.Contains(rp.PermissionId)).ToList();

        _context.RolePermissions.RemoveRange(permissionsToRemove);
        return _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateRole(Guid roleId, UpdateRoleDTO update)
    {
        var role = await _context.Roles
            .Where(r => !r.IsDeleted).FirstOrDefaultAsync(r => r.Id == roleId);
        if (role == null)
            throw new NotFoundException(ErrorCodes.Roles.ROLE_NOT_FOUND, $"Role with id '{roleId}' was not found.");

        if (string.IsNullOrEmpty(update.Name))
            throw new BadRequestException("role name required", ErrorCodes.Roles.ROLE_NAME_REQUIRED);

        role.Name = update.Name;
        role.Description = update.Description;

        await _context.SaveChangesAsync();
        return true;
    }

    private RoleDto _MapToRoleDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            PermissionsCount = role.RolePermissions.Count,
            UsersCount = role.UserRoles.Count
        };
    }
}