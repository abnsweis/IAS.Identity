namespace IAS.Identity.Domain.Common.Options;

public class PasswordValidator
{
    private readonly PasswordOptions _options;

    public PasswordValidator(PasswordOptions options)
    {
        this._options = options;
    }

    public PasswordValidationResult Validate(string password)
    {
        var errors = new List<PasswordError>();

        if (string.IsNullOrEmpty(password) || password.Length < _options.MinLength)
        {
            errors.Add(new PasswordError($"Password must be at least {_options.MinLength} characters long.", "PASSWORD_TOO_SHORT"));
        }
        if (_options.RequireUppercase && !password.Any(char.IsUpper))
        {
            errors.Add(new PasswordError("Password must contain at least one uppercase letter.", "PASSWORD_REQUIRE_UPPERCASE"));
        }

        if (_options.RequireLowercase && !password.Any(char.IsLower))
        {
            errors.Add(new PasswordError("Password must contain at least one lowercase letter.", "PASSWORD_REQUIRE_LOWERCASE"));
        }

        if (_options.RequireDigit && !password.Any(char.IsDigit))
        {
            errors.Add(new PasswordError("Password must contain at least one digit.", "PASSWORD_REQUIRE_DIGIT"));
        }

        if (_options.RequireSpecialChar && !password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            errors.Add(new PasswordError("Password must contain at least one special character.", "PASSWORD_REQUIRE_SPECIAL_CHAR"));
        }

        return errors.Count == 0
            ? PasswordValidationResult.Success() : PasswordValidationResult.Failure(errors);
    }
}