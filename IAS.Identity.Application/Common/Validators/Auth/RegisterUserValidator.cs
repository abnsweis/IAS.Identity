using FluentValidation;
using IAS.Identity.Application.Common.Dtos.Users;
using IAS.Identity.Application.Common.Validators.Password;

namespace IAS.Identity.Application.Common.Validators.Auth;

public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator(PasswordFluentValidator passwordFluentValidator)
    {
        // Name
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Name is required.")
                .WithErrorCode(ErrorCodes.Users.NAME_REQUIRED)
            .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.")
                .WithErrorCode(ErrorCodes.Users.NAME_MAX_LENGTH);

        // Username
        RuleFor(x => x.Username)
            .NotEmpty()
                .WithMessage("Username is required.")
                .WithErrorCode(ErrorCodes.Users.USERNAME_REQUIRED)
            .MinimumLength(6)
                .WithMessage("Username must be at least 6 characters.")
                .WithErrorCode(ErrorCodes.Users.USERNAME_MIN_LENGTH)
            .MaximumLength(50)
                .WithMessage("Username must not exceed 50 characters.")
                .WithErrorCode(ErrorCodes.Users.USERNAME_MAX_LENGTH)
            .Matches("^[a-zA-Z0-9@._-]+$")
                .WithMessage("Username can only contain letters, numbers, and the following characters: @ . _ -")
                .WithErrorCode(ErrorCodes.Users.USERNAME_INVALID_FORMAT);

        // Email
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email is required.")
                .WithErrorCode(ErrorCodes.Users.EMAIL_REQUIRED)
            .EmailAddress()
                .WithMessage("Valid email address is required.")
                .WithErrorCode(ErrorCodes.Users.EMAIL_INVALID)
            .MaximumLength(100)
                .WithMessage("Email must not exceed 100 characters.")
                .WithErrorCode(ErrorCodes.Users.EMAIL_MAX_LENGTH);

        // Password
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .WithErrorCode(ErrorCodes.Users.PASSWORD_REQUIRED)
            .SetValidator(passwordFluentValidator);

        // Confirm password validation
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match")
            .WithErrorCode(ErrorCodes.Auth.PASSWORD_MISMATCH);

        // Phone (optional field)
        RuleFor(x => x.Phone)
            .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 characters.")
                .WithErrorCode(ErrorCodes.Users.PHONE_MAX_LENGTH)
            .Matches(@"^\+?[0-9\s\-\(\)]+$")
                .When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("Phone number contains invalid characters.")
                .WithErrorCode(ErrorCodes.Users.PHONE_INVALID_FORMAT);
    }
}