using IAS.Identity.Domain.Entities;
using IAS.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAS.Identity.Infrastructure.Persistence.Seed;

internal class UserSeeder
{
    public static async Task AddUsersAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var superAdminId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var AdminId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var userId = Guid.Parse("30000000-0000-0000-0000-000000000003");

        var superAdmin = _AddNewUser(superAdminId, "Super Admin", "superadmin.IAS.com", "1234567890", "superadmin", "superadminpassword");
        var admin = _AddNewUser(AdminId, "Admin", "admin.IAS.com", "1234567891", "admin", "adminpassword");
        var user = _AddNewUser(userId, "User", "user.IAS.com", "1234567892", "user", "userpassword");

        await context.Users.AddRangeAsync(superAdmin, admin, user);
        await context.SaveChangesAsync();
    }

    private static User _AddNewUser(Guid Id, string name, string email, string phone, string username, string password)
    {
        return new User
        {
            Id = Id,
            Name = name,
            Email = email,
            Phone = phone,
            Username = username,
            PasswordHash = password,
        };
    }
}