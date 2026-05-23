namespace YourApp.Infrastructure.Exceptions;

/// <summary>Base class for all domain/application exceptions.</summary>
public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected AppException(string message, int statusCode, string errorCode)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}