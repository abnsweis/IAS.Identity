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
                // Ensure TraceId is always injected even for non-exception responses
                ctx.ProblemDetails.Extensions.TryAdd("traceId", ctx.HttpContext.TraceIdentifier);
                ctx.ProblemDetails.Extensions.TryAdd("timestamp", DateTime.UtcNow);
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