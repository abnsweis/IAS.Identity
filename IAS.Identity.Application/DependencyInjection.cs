using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using IAS.Identity.Application.Common.Validators.Users;
using IAS.Identity.Application.Common.Validators.Password;
using IAS.Identity.Domain.Common.Options;
using IAS.Identity.Application.Common.Models;

namespace IAS.Identity.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
            services.AddSingleton<PasswordFluentValidator>();
        }
    }
}