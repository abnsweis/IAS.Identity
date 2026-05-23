using IAS.Identity.Domain.Common.Constants;
using IAS.Identity.Domain.Entities;
using IAS.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAS.Identity.Infrastructure.Persistence.Seed;

/// <summary>
/// Provides functionality to seed role-permission relationships into the database
/// </summary>
internal class RolePermissionSeeder
{
    /// <summary>
    /// Seeds predefined role-permission assignments into the database
    /// </summary>
    /// <param name="context">The database context to use for seeding</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method creates role-permission relationships based on predefined roles and their permissions
    /// </remarks>
    public static async Task SeedRolePermissionsAsync(ApplicationDbContext context)
    {
        if (await context.RolePermissions.AnyAsync()) return;

        // Get all existing permissions
        var allPermissions = await context.Permissions.ToListAsync();

        // Get or create default roles
        var superAdminRole = await GetOrCreateRole(context, Roles.SuperAdmin, "Full system access");
        var AdminRole = await GetOrCreateRole(context, Roles.Admin, "Management level access");
        var userRole = await GetOrCreateRole(context, Roles.User, "Basic user access");

        // Assign permissions to roles
        await AssignPermissionsToSuperAdminRole(context, superAdminRole.Id, allPermissions);
        await AssignPermissionsToAdminRole(context, AdminRole.Id, allPermissions);
        await AssignPermissionsToUserRole(context, userRole.Id, allPermissions);

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets an existing role by name or creates a new one if it doesn't exist
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="roleName">Name of the role</param>
    /// <param name="description">Description of the role</param>
    /// <returns>The existing or newly created role</returns>
    private static async Task<Role> GetOrCreateRole(ApplicationDbContext context, string roleName, string description)
    {
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
        {
            role = new Role
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                Description = description,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
        }

        return role;
    }

    /// <summary>
    /// Assigns all permissions to the SuperAdmin role
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="roleId">The ID of the SuperAdmin role</param>
    /// <param name="allPermissions">List of all available permissions</param>
    private static async Task AssignPermissionsToSuperAdminRole(ApplicationDbContext context, Guid roleId, List<Permission> allPermissions)
    {
        foreach (var permission in allPermissions)
        {
            var existingMapping = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

            if (existingMapping == null)
            {
                await context.RolePermissions.AddAsync(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id,
                    GrantedAt = DateTimeOffset.UtcNow
                });
            }
        }
    }

    /// <summary>
    /// Assigns manager-level permissions to the Manager role
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="roleId">The ID of the Manager role</param>
    /// <param name="allPermissions">List of all available permissions</param>
    private static async Task AssignPermissionsToAdminRole(ApplicationDbContext context, Guid roleId, List<Permission> allPermissions)
    {
        var managerPermissions = allPermissions.Where(p =>
            p.Name.Contains("users.read") ||
            p.Name.Contains("users.create") ||
            p.Name.Contains("users.update") ||
            p.Name.Contains("roles.read")
        ).ToList();

        foreach (var permission in managerPermissions)
        {
            var existingMapping = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

            if (existingMapping == null)
            {
                await context.RolePermissions.AddAsync(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow
                });
            }
        }
    }

    /// <summary>
    /// Assigns basic read-only permissions to the User role
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="roleId">The ID of the User role</param>
    /// <param name="allPermissions">List of all available permissions</param>
    private static async Task AssignPermissionsToUserRole(ApplicationDbContext context, Guid roleId, List<Permission> allPermissions)
    {
        var userPermissions = allPermissions.Where(p =>
            p.Name == "users.read" ||
            p.Name == "roles.read" ||
            p.Name == "permissions.read"
        ).ToList();

        foreach (var permission in userPermissions)
        {
            var existingMapping = await context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

            if (existingMapping == null)
            {
                await context.RolePermissions.AddAsync(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow
                });
            }
        }
    }
}