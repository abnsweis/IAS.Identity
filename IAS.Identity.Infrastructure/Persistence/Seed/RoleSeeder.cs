using IAS.Identity.Domain.Common.Constants;
using IAS.Identity.Domain.Entities;
using IAS.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAS.Identity.Infrastructure.Persistence.Seed;

internal class RoleSeeder
{
    public static async Task AddRolesAsync(ApplicationDbContext context)
    {
        if (await context.Roles.AnyAsync()) return;

        var superAdminRole = new Role
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Name = Roles.SuperAdmin,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var adminRole = new Role
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
            Name = Roles.Admin,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var userRole = new Role
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
            Name = Roles.User,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await context.Roles.AddRangeAsync(superAdminRole, adminRole, userRole);
        await context.SaveChangesAsync();
    }
}