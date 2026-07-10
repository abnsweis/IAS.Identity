namespace IAS.Identity.Application.Common.Exceptions;

/// <summary>403 – User is authenticated but not allowed.</summary>
public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message, 403, "FORBIDDEN") { }
}