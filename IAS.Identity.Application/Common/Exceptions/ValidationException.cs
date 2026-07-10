namespace IAS.Identity.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new List<string>();
    }

    public ValidationException(List<string> errors) : base("One or more validation errors occurred.")
    {
        this.Errors = errors;
    }

    public ValidationException(string message, List<string> errors) : base(message)
    {
        this.Errors = errors;
    }
}