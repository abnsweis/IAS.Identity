using FluentValidation;
using IAS.Identity.API.Infrastructure;
using IAS.Identity.API.Infrastructure.Filters;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Application.Common.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace IAS.Identity.API
{
    public static class DependencyInjection
    {
        public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(ValidationFilter<>));
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUserService>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => new { errorCode = "VALIDATION_ERROR", message = e.ErrorMessage })
                        );

                    return new BadRequestObjectResult(new
                    {
                        status = 400,
                        title = "One or more validation errors occurred.",
                        errors
                    });
                };
            });

            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new();

                    document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                    {
                        ["Bearer"] = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            In = ParameterLocation.Header,
                            Description = "Enter JWT Token"
                        }
                    };

                    document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });

                    return Task.CompletedTask;
                });
            });
            services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
                };
            });

            services.AddAuthorization();
        }
    }
}