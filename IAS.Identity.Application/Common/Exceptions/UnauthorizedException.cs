// ============================================================
//  CustomExceptionHandler.cs
//  ASP.NET Core Web API — Global Exception Handler
//  Using RFC 7807 ProblemDetails standard
//  Compatible with .NET 8+
// ============================================================

namespace YourApp.Infrastructure.Exceptions;

/// <summary>401 – User is not authenticated.</summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Authentication is required.")
        : base(message, 401, "UNAUTHORIZED") { }
}
