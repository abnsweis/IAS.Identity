// ============================================================
//  CustomExceptionHandler.cs
//  ASP.NET Core Web API — Global Exception Handler
//  Using RFC 7807 ProblemDetails standard
//  Compatible with .NET 8+
// ============================================================

namespace YourApp.Infrastructure.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string resource, object key, string code = "NOT_FOUND")
        : base($"{resource} with id '{key}' was not found.", 404, code) { }

    public NotFoundException(string message, string code = "NOT_FOUND")
        : base(message, 404, code) { }
}