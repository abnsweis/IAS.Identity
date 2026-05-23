// ============================================================
//  CustomExceptionHandler.cs
//  ASP.NET Core Web API — Global Exception Handler
//  Using RFC 7807 ProblemDetails standard
//  Compatible with .NET 8+
// ============================================================

namespace YourApp.Infrastructure.Exceptions;

// ─────────────────────────────────────────────────────────────
//  4. Helper – HTTP Reason Phrases
// ─────────────────────────────────────────────────────────────

internal static class ReasonPhrases
{
    private static readonly Dictionary<int, string> _phrases = new()
    {
        { 400, "Bad Request" },          { 401, "Unauthorized" },
        { 403, "Forbidden" },            { 404, "Not Found" },
        { 408, "Request Timeout" },      { 409, "Conflict" },
        { 422, "Unprocessable Entity" }, { 500, "Internal Server Error" }
    };

    public static string GetReasonPhrase(int statusCode)
        => _phrases.TryGetValue(statusCode, out var phrase) ? phrase : "Error";
}
