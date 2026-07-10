namespace IAS.Identity.API.Infrastructure;

public static class ExceptionHandlerExtensions
{
    /// <summary>Registers GlobalExceptionHandler + ProblemDetails services.</summary>
    public static IServiceCollection AddGlobalExceptionHandler(
        this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Configures IProblemDetailsService with custom AppProblemDetails serialization
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
            };
        });

        return services;
    }

    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        return app;
    }
}