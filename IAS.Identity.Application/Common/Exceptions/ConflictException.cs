namespace IAS.Identity.Application.Common.Exceptions;

/// <summary>409 – Conflict (e.g., duplicate resource).</summary>
public class ConflictException : AppException
{
    public ConflictException(string message, string code)
        : base(message, 409, code) { }
}