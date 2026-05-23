using IAS.Identity.API.Infrastructure;
using IAS.Identity.Application;
using IAS.Identity.Infrastructure;
using IAS.Identity.Infrastructure.Data;
using IAS.Identity.Infrastructure.Persistence.Seed;
using Scalar.AspNetCore;
using Mapster;

namespace IAS.Identity.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddApiServices();
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddGlobalExceptionHandler();

            var app = builder.Build();

            app.UseGlobalExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapScalarApiReference();
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await DatabaseSeeder.SeedAll(dbContext);
            }
            app.Run();
        }
    }
}