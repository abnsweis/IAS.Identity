// ============================================================
//  CustomExceptionHandler.cs
//  ASP.NET Core Web API — Global Exception Handler
//  Using RFC 7807 ProblemDetails standard
//  Compatible with .NET 8+
// ============================================================

namespace YourApp.Infrastructure.Exceptions;

/// <summary>422 – Unprocessable entity.</summary>
public class UnprocessableEntityException : AppException
{
    public UnprocessableEntityException(string message)
        : base(message, 422, "UNPROCESSABLE_ENTITY") { }
}
