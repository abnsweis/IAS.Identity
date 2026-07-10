namespace IAS.Identity.Application.Common.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string resource, object key, string code = "NOT_FOUND")
        : base($"{resource} with id '{key}' was not found.", 404, code) { }

    public NotFoundException(string message, string code = "NOT_FOUND")
        : base(message, 404, code) { }
}