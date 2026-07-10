using IAS.Identity.Domain.Common.Options;
using Microsoft.Extensions.DependencyInjection;

namespace IAS.Identity.Infrastructure.Extensions;

public static class Extensions
{
    public static IServiceCollection Add_IASIdentity(this IServiceCollection services, Action<CustomIASIdentityOptions> configureOptions)
    {
        var options = new CustomIASIdentityOptions();
        configureOptions(options);

        services.AddSingleton(options.Password);
        services.AddSingleton<PasswordValidator>();
        return services;
    }
}