using IAS.Identity.Application.Common;
using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Application.Common.Models;
using IAS.Identity.Domain.Entities;
using IAS.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using YourApp.Infrastructure.Exceptions;

namespace IAS.Identity.Infrastructure.Services;

internal class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
        this._context = context;
    }

    public async Task<RoleDto> CreateRole(CreateAndUpdateRole create)
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

    public Task<PaginatedList<RoleDto>> GetPaginatedListRoles(int pageNumber, int pageSize)
    {
        var roleDtos = _context.Roles.Include(r => r.UserRoles).Include(r => r.RolePermissions).Select(_MapToRoleDto).AsQueryable();

        var paginatedRoles = PaginatedList<RoleDto>.CreateAsync(roleDtos, pageNumber, pageSize);

        return paginatedRoles;
    }

    public async Task<RoleDto> GetRoleById(Guid roleId)
    {
        var role = await _context.Roles.Include(r => r.UserRoles).Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == roleId);
        if (role == null)
            throw new NotFoundException(ErrorCodes.Roles.ROLE_NOT_FOUND, $"Role with id '{roleId}' was not found.");
        return _MapToRoleDto(role);
    }

    public Task<List<RoleLookUpDto>> GetRoleLookUpDtos()
    {
        var roleLookUpDtos = _context.Roles.Select(r => new RoleLookUpDto
        {
            RoleId = r.Id,
            Name = r.Name
        }).ToListAsync();
        return roleLookUpDtos;
    }

    public async Task<bool> UpdateRole(CreateAndUpdateRole dto)
    {
        var role = await _context.Roles.FindAsync(dto.RoleId);
        if (role == null)
            throw new NotFoundException(ErrorCodes.Roles.ROLE_NOT_FOUND, $"Role with id '{dto.RoleId}' was not found.");
        if (string.IsNullOrEmpty(dto.Name))
            throw new BadRequestException("role name required", ErrorCodes.Roles.ROLE_NAME_REQUIRED);

        role.Name = dto.Name;
        role.Description = dto.Description;

        await _context.SaveChangesAsync();
        return true;
    }

    private RoleDto _MapToRoleDto(Role role)
    {
        return new RoleDto
        {
            Name = role.Name,
            Description = role.Description,
            PermissionsCount = role.RolePermissions.Count,
            UsersCount = role.UserRoles.Count
        };
    }
}