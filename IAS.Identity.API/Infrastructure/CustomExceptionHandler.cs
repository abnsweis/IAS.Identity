using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using YourApp.Infrastructure.Exceptions;

namespace IAS.Identity.API.Infrastructure;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService,
    IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        var (statusCode, errorCode, title, message, validationErrors)
            = MapException(exception);

        // ── Logging ──────────────────────────────────────────
        if (statusCode >= 500)
            logger.LogError(exception,
                "[{TraceId}] Unhandled exception | {Method} {Path}",
                traceId, httpContext.Request.Method, httpContext.Request.Path);
        else
            logger.LogWarning(
                "[{TraceId}] Handled exception ({ErrorCode}): {Message}",
                traceId, errorCode, message);

        // ── Build AppProblemDetails ───────────────────────────
        var problemDetails = new AppProblemDetails
        {
            // Standard RFC 7807 fields
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = message,
            Instance = httpContext.Request.Path,

            // Custom extensions
            ErrorCode = errorCode,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow,
            Errors = validationErrors,
            Details = env.IsDevelopment() ? exception.ToString() : null
        };

        httpContext.Response.StatusCode = statusCode;

        // Use IProblemDetailsService to write with proper content negotiation
        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        return true;
    }

    // ── Exception → ProblemDetails mapping ───────────────────
    private static (int status, string code, string title, string message,
                    IDictionary<string, string[]>? errors)
        MapException(Exception ex) => ex switch
        {
            BadRequestException { ValidationErrors: not null } bre => (
                400, bre.ErrorCode,
                "Validation Error",
                bre.Message,
                bre.ValidationErrors),

            AppException app => (
                app.StatusCode, app.ErrorCode,
                ReasonPhrases.GetReasonPhrase(app.StatusCode),
                app.Message, null),

            OperationCanceledException =>
                (499, "REQUEST_CANCELLED", "Request Cancelled",
                 "The request was cancelled by the client.", null),

            KeyNotFoundException =>
                (404, "NOT_FOUND", "Not Found", ex.Message, null),

            UnauthorizedAccessException =>
                (401, "UNAUTHORIZED", "Unauthorized",
                 "Authentication is required.", null),

            ArgumentNullException or ArgumentException =>
                (400, "BAD_ARGUMENT", "Bad Request", ex.Message, null),

            InvalidOperationException =>
                (422, "INVALID_OPERATION", "Unprocessable Entity", ex.Message, null),

            TimeoutException =>
                (408, "REQUEST_TIMEOUT", "Request Timeout",
                 "The operation timed out.", null),

            _ => (500, "INTERNAL_SERVER_ERROR", "Internal Server Error",
                  "An unexpected error occurred. Please try again later.", null)
        };
}