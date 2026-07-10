namespace IAS.Identity.Application.Common.Exceptions;

/// <summary>401 – User is not authenticated.</summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Authentication is required.")
        : base(message, 401, ErrorCodes.Auth.USER_NOT_AUTHENTICATED) { }
}