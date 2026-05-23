using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Infrastructure.Data;
using IAS.Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IAS.Identity.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the ApplicationDbContext with the dependency injection container
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        // Register other infrastructure services here if needed

        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
    }
}