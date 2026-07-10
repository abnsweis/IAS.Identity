using IAS.Identity.Application.Common.Exceptions;
using IAS.Identity.Domain.Common.Exceptions;
using IAS.Identity.Domain.Common.Exceptions.Users;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;

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
             400, bre.ErrorCode, "Validation Error",
             bre.Message, bre.ValidationErrors),

         UsernameAlreadyExistsException e =>
             (409, "USERNAME_ALREADY_EXISTS", "Conflict", e.Message, null),

         DomainException e =>
             (422, "DOMAIN_RULE_VIOLATED", "Business Rule Violation", e.Message, null),
         ForbiddenException e =>
             (403, "FORBIDDEN", "Forbidden", $"Access forbidden: {e.Message}", null),
         // ── AppException ──────────────────────────────────────
         AppException app => (
             app.StatusCode, app.ErrorCode,
             ReasonPhrases.GetReasonPhrase(app.StatusCode),
             app.Message, null),

         // ── System ───────────────────────────────────────────
         OperationCanceledException =>
             (499, "REQUEST_CANCELLED", "Request Cancelled",
              "The request was cancelled.", null),

         _ => (500, "INTERNAL_SERVER_ERROR", "Internal Server Error",
               "An unexpected error occurred.", null)
     };
}