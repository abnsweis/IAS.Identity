using Microsoft.AspNetCore.Mvc;

namespace IAS.Identity.API.Infrastructure;

public sealed class AppProblemDetails : ProblemDetails
{
    public string ErrorCode { get; set; } = default!;
    public string TraceId { get; set; } = default!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public IDictionary<string, string[]>? Errors { get; set; }
    public string? Details { get; set; }
}