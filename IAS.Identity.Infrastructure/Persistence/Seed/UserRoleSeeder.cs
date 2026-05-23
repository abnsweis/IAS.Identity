using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IAS.Identity.Domain.Entities;
using IAS.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAS.Identity.Infrastructure.Persistence.Seed;

/// <summary>
/// Provides functionality to seed user-role relationships into the database
/// </summary>
internal class UserRoleSeeder
{
    /// <summary>
    /// Seeds predefined user-role assignments into the database
    /// </summary>
    /// <param name="context">The database context to use for seeding</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method assigns users to appropriate roles based on predefined relationships
    /// </remarks>
    public static async Task SeedUserRolesAsync(ApplicationDbContext context)
    {
        if (await context.UserRoles.AnyAsync()) return;

        // Get all existing users (using the same IDs from UserSeeder)

        // SuperAdmin
        var superAdmin = await context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse("10000000-0000-0000-0000-000000000001"));
        // Admin
        var admin = await context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse("20000000-0000-0000-0000-000000000002"));
        // Regular User
        var regularUser = await context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse("30000000-0000-0000-0000-000000000003"));

        // Get all existing roles
        var superAdminRole = await context.Roles
            .FirstOrDefaultAsync(r => r.Name == "SuperAdmin");

        var adminRole = await context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Admin");

        var userRole = await context.Roles
            .FirstOrDefaultAsync(r => r.Name == "User");

        // Assign roles to users
        if (superAdmin != null && superAdminRole != null)
        {
            await AssignUserToRole(context, superAdmin.Id, superAdminRole.Id);
        }

        if (admin != null && adminRole != null)
        {
            await AssignUserToRole(context, admin.Id, adminRole.Id);
        }

        if (regularUser != null && userRole != null)
        {
            await AssignUserToRole(context, regularUser.Id, userRole.Id);
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Assigns a user to a specific role
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="userId">The user identifier</param>
    /// <param name="roleId">The role identifier</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private static async Task AssignUserToRole(ApplicationDbContext context, Guid userId, Guid roleId)
    {
        var existingAssignment = await context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (existingAssignment == null)
        {
            await context.UserRoles.AddAsync(new UserRole
            {
                UserId = userId,
                RoleId = roleId,
            });
        }
    }
}