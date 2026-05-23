using IAS.Identity.Domain.Entities;
using IAS.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAS.Identity.Infrastructure.Persistence.Seed;

/// <summary>
/// Provides functionality to seed predefined permissions into the database
/// </summary>
internal class PermissionSeeder
{
    /// <summary>
    /// Seeds all permission groups (Users, Roles, Permissions) into the database
    /// </summary>
    /// <param name="context">The database context to use for seeding</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method combines all permission groups and adds them to the database in a single transaction
    /// </remarks>
    public static async Task AddPermissionsAsync(ApplicationDbContext context)
    {
        if (await context.Permissions.AnyAsync()) return;
        var allPermissions = new List<Permission>();

        allPermissions.AddRange(GetUserManagementPermissions());
        allPermissions.AddRange(GetRoleManagementPermissions());
        allPermissions.AddRange(GetPermissionManagementPermissions());

        await context.Permissions.AddRangeAsync(allPermissions);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new Permission entity with the specified identifier and name
    /// </summary>
    /// <param name="id">The unique identifier (GUID) for the permission</param>
    /// <param name="name">The name of the permission (e.g., "users.read")</param>
    /// <returns>A new Permission instance with the specified properties</returns>
    /// <remarks>
    /// This is a factory method for creating Permission objects without database context
    /// </remarks>
    private static Permission CreatePermission(Guid id, string name)
    {
        return new Permission
        {
            Id = id,
            Name = name,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Retrieves a list of permissions related to user management operations
    /// </summary>
    /// <returns>A list containing CRUD permissions for user management</returns>
    /// <remarks>
    /// Includes permissions:
    /// - users.read: View user information
    /// - users.create: Create new users
    /// - users.update: Modify existing users
    /// - users.delete: Remove users from the system
    /// </remarks>
    private static List<Permission> GetUserManagementPermissions()
    {
        return new List<Permission>
        {
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000001"), "users.read"),
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000002"), "users.create"),
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000003"), "users.update"),
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000004"), "users.delete")
        };
    }

    /// <summary>
    /// Retrieves a list of permissions related to role management operations
    /// </summary>
    /// <returns>A list containing CRUD permissions for role management</returns>
    /// <remarks>
    /// Includes permissions:
    /// - roles.read: View role information
    /// - roles.create: Create new roles
    /// - roles.update: Modify existing roles
    /// - roles.delete: Remove roles from the system
    /// </remarks>
    private static List<Permission> GetRoleManagementPermissions()
    {
        return new List<Permission>
        {
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000005"), "roles.read"),
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000006"), "roles.create"),
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000007"), "roles.update"),
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000008"), "roles.delete")
        };
    }

    /// <summary>
    /// Retrieves a list of permissions related to permission management operations
    /// </summary>
    /// <returns>A list containing CRUD permissions for permission management</returns>
    /// <remarks>
    /// Includes permissions:
    /// - permissions.read: View permission information
    /// - permissions.create: Create new permissions
    /// - permissions.update: Modify existing permissions
    /// - permissions.delete: Remove permissions from the system
    /// </remarks>
    private static List<Permission> GetPermissionManagementPermissions()
    {
        return new List<Permission>
        {
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000009"), "permissions.read"),
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000010"), "permissions.create"),
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000011"), "permissions.update"),
            CreatePermission(Guid.Parse("40000000-0000-0000-0000-000000000012"), "permissions.delete")
        };
    }
}