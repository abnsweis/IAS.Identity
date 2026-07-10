using IAS.Identity.Application.Common;
using IAS.Identity.Application.Common.Dtos.Permissions;
using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Dtos.Users;
using IAS.Identity.Application.Common.Exceptions;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Application.Common.Models;
using IAS.Identity.Domain.Entities;
using IAS.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _context;

    public PermissionService(ApplicationDbContext context)
    {
        this._context = context;
    }

    public async Task<PermissionDetailsDTO> CreatePermission(CreatePermissionRequest create)
    {
        if (await _context.Permissions.AnyAsync(p => p.Name == create.Name))
            throw new BadRequestException($"Permission with name '{create.Name}' already exists.", ErrorCodes.Permissions.PERMISSION_NAME_ALREADY_EXISTS);

        var newPermission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = create.Name,
            Description = create.Description
        };

        await _context.Permissions.AddAsync(newPermission);
        await _context.SaveChangesAsync();

        return new PermissionDetailsDTO
        {
            Id = newPermission.Id,
            Name = newPermission.Name,
            Description = newPermission.Description,
            RoleNames = new List<RoleDto>(),
            UserNames = new List<UserSummaryDTO>()
        };
    }

    public async Task<bool> DeletePermissionAsync(Guid permissionId)
    {
        var permission = _context.Permissions.FirstOrDefault(p => p.Id == permissionId);
        if (permission is null)
            throw new NotFoundException($"Permission with id '{permissionId}' was not found.", ErrorCodes.Permissions.PERMISSION_NOT_FOUND);

        permission.Delete();

        _context.Permissions.Update(permission);
        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
            throw new InternalServerErrorException($"Failed to delete permission with id '{permissionId}'.");
        return result;
    }

    public async Task<PaginatedList<PermissionSummaryDTO>> GetPaginatedListPermissions(int pageNumber, int pageSize)
    {
        var permissions = _context.Permissions
            .Select(p => new PermissionSummaryDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .AsQueryable();

        return await PaginatedList<PermissionSummaryDTO>.CreateAsync(permissions, pageNumber, pageSize);
    }

    public async Task<PermissionDetailsDTO> GetPermissionById(Guid permissionId)
    {
        var permission = await _context.Permissions
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .Include(p => p.RolePermissions)
                .ThenInclude(rp => rp.Role)
            .FirstOrDefaultAsync(p => p.Id == permissionId);

        if (permission is null)
            throw new NotFoundException($"Permission with id '{permissionId}' was not found.", ErrorCodes.Permissions.PERMISSION_NOT_FOUND);

        var permissionDTO = new PermissionDetailsDTO
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            RoleNames = permission.RolePermissions.Select(rp => new RoleDto
            {
                Id = rp.RoleId,
                Name = rp.Role.Name,
                Description = rp.Role.Description
            }).ToList(),

            UserNames = permission.RolePermissions
                .SelectMany(rp => rp.Role.UserRoles)
                .Select(ur => ur.User)
                .Distinct()
                .Select(u => new UserSummaryDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                })
                .ToList()
        };

        return permissionDTO;
    }

    public async Task<bool> UpdatePermission(Guid permissionId, UpdatePermissionRequest update)
    {
        var permission = _context.Permissions.FirstOrDefault(p => p.Id == permissionId);
        if (permission is null)
            throw new NotFoundException($"Permission with id '{permissionId}' was not found.", ErrorCodes.Permissions.PERMISSION_NOT_FOUND);

        if (await _context.Permissions.AnyAsync(p => p.Name == update.Name && p.Id != permissionId))
            throw new ConflictException(
                $"A permission with the name '{update.Name}' already exists. Please use a different name.",
                ErrorCodes.Permissions.PERMISSION_NAME_CONFLICT
            );

        permission.Name = update.Name;
        permission.Description = update.Description;

        _context.Permissions.Update(permission);
        await _context.SaveChangesAsync();
        return true;
    }
}