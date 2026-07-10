namespace IAS.Identity.Domain.Common.Options;

public sealed record PasswordError(string Message, string Code);

public class PasswordValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public IReadOnlyList<PasswordError> Errors { get; init; } = [];

    public static PasswordValidationResult Success() => new() { Errors = [] };

    public static PasswordValidationResult Failure(IEnumerable<PasswordError> errors)
    {
        return new PasswordValidationResult { Errors = errors.ToList() };
    }
}