using IAS.Identity.API.Infrastructure;
using IAS.Identity.Application;
using IAS.Identity.Infrastructure;
using IAS.Identity.Infrastructure.Data;
using IAS.Identity.Infrastructure.Persistence.Seed;
using Scalar.AspNetCore;
using Mapster;
using FluentValidation;
using IAS.Identity.Application.Common.Dtos.Users;
using Microsoft.Extensions.DependencyInjection;
using IAS.Identity.Application.Common.Converters;
using IAS.Identity.Application.Common.Models;
using IAS.Identity.Application.Common.Interface;

namespace IAS.Identity.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new FlexibleEnumConverterFactory());
                });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddApiServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddGlobalExceptionHandler();
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings")
            );

            var app = builder.Build();

            app.UseGlobalExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapScalarApiReference();
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var passwordHasher =
                    scope.ServiceProvider
                         .GetRequiredService<IPasswordHasherService>();
                await dbContext.Database.EnsureCreatedAsync();
                await DatabaseSeeder.SeedAll(dbContext, passwordHasher);

                // ›Ì √Ì „ﬂ«‰ ⁄‰œﬂ IServiceProvider
                var validator = scope.ServiceProvider.GetService<IValidator<CreateUserDto>>();

                if (validator is null)
                    throw new Exception("CreateUserDtoValidator is not registered!");
            }
            app.Run();
        }
    }
}