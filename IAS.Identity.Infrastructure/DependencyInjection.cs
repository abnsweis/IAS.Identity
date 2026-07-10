using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Application.Common.Models;
using IAS.Identity.Infrastructure.Data;
using IAS.Identity.Infrastructure.Extensions;
using IAS.Identity.Infrastructure.Persistence.Validators;
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
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        // Register the IUserUniquenessValidator with the dependency injection container
        services.AddScoped<IUserUniquenessValidator, UserUniquenessValidator>();
        services.Add_IASIdentity(options =>
        {
            options.Password.MinLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireSpecialChar = true;
        });
    }
}