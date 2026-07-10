namespace IAS.Identity.Application.Common.Exceptions;

/// <summary>422 – Unprocessable entity.</summary>
public class UnprocessableEntityException : AppException
{
    public UnprocessableEntityException(string message, string code)
        : base(message, 422, code) { }
}