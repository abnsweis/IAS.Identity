using System.Threading.Tasks;
using IAS.Identity.Infrastructure.Data;

namespace IAS.Identity.Infrastructure.Persistence.Seed;

/// <summary>
/// Main seeder that orchestrates all seeding operations
/// </summary>
public class DatabaseSeeder
{
    /// <summary>
    /// Seeds all data including users, permissions, role-permissions, and user-roles
    /// </summary>
    /// <param name="context">The database context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task SeedAll(ApplicationDbContext context)
    {
        // First seed users
        await UserSeeder.AddUsersAsync(context);

        // Then seed roles
        await RoleSeeder.AddRolesAsync(context);

        // Then seed permissions
        await PermissionSeeder.AddPermissionsAsync(context);

        // Then seed role-permission relationships
        await RolePermissionSeeder.SeedRolePermissionsAsync(context);

        // Finally seed user-role relationships
        await UserRoleSeeder.SeedUserRolesAsync(context);
    }
}