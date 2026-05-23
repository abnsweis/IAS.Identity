// ============================================================
//  CustomExceptionHandler.cs
//  ASP.NET Core Web API — Global Exception Handler
//  Using RFC 7807 ProblemDetails standard
//  Compatible with .NET 8+
// ============================================================

namespace YourApp.Infrastructure.Exceptions;

/// <summary>400 – Business / validation rule violated.</summary>
public class BadRequestException : AppException
{
    public IDictionary<string, string[]>? ValidationErrors { get; }

    public BadRequestException(string message, string code)
        : base(message, 400, code) { }

    public BadRequestException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", 400, "VALIDATION_ERROR")
        => ValidationErrors = errors;
}