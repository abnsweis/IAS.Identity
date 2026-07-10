namespace IAS.Identity.Infrastructure.Persistence.Validators;

using global::IAS.Identity.Application.Common.Dtos.Users;
using global::IAS.Identity.Application.Common.Interface;
using global::IAS.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementation of user uniqueness validation using Entity Framework.
/// </summary>
public class UserUniquenessValidator : IUserUniquenessValidator
{
    private readonly ApplicationDbContext _context;

    public UserUniquenessValidator(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Validates all unique fields for a new user.
    /// </summary>
    public async Task<Dictionary<string, string>> ValidateUniquenessAsync(
        CreateUserDto dto,
        CancellationToken cancellationToken = default)
    {
        var errors = new Dictionary<string, string>();

        // Validate Name
        if (!await IsNameUniqueAsync(dto.Name, cancellationToken))
            errors.Add("Name", "Name already exists in the system");

        // Validate Username
        if (!await IsUsernameUniqueAsync(dto.Username, cancellationToken))
            errors.Add("Username", "Username already taken");

        // Validate Email
        if (!await IsEmailUniqueAsync(dto.Email, cancellationToken))
            errors.Add("Email", "Email already registered");

        // Validate Phone (if provided)
        if (!string.IsNullOrEmpty(dto.Phone) &&
            !await IsPhoneUniqueAsync(dto.Phone, cancellationToken))
            errors.Add("Phone", "Phone number already in use");

        return errors;
    }

    /// <summary>
    /// Checks if a name is unique (case-insensitive).
    /// </summary>
    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default)
    {
        return !await _context.Users
            .AnyAsync(u => u.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    /// <summary>
    /// Checks if a username is unique.
    /// </summary>
    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default)
    {
        return !await _context.Users
            .AnyAsync(u => u.Username == username, cancellationToken);
    }

    /// <summary>
    /// Checks if an email is unique (case-insensitive).
    /// </summary>
    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        return !await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    /// <summary>
    /// Checks if a phone number is unique.
    /// </summary>
    public async Task<bool> IsPhoneUniqueAsync(string phone, CancellationToken cancellationToken = default)
    {
        return !await _context.Users
            .AnyAsync(u => u.Phone == phone, cancellationToken);
    }

    /// <summary>
    /// Checks if a name is unique excluding a specific user (for updates).
    /// </summary>
    public async Task<bool> IsNameUniqueForUpdateAsync(
        string name,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return !await _context.Users
            .AnyAsync(u => u.Id != userId &&
                          u.Name.ToLower() == name.ToLower(),
                      cancellationToken);
    }

    /// <summary>
    /// Checks if a username is unique excluding a specific user (for updates).
    /// </summary>
    public async Task<bool> IsUsernameUniqueForUpdateAsync(
        string username,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return !await _context.Users
            .AnyAsync(u => u.Id != userId &&
                          u.Username == username,
                      cancellationToken);
    }

    /// <summary>
    /// Checks if an email is unique excluding a specific user (for updates).
    /// </summary>
    public async Task<bool> IsEmailUniqueForUpdateAsync(
        string email,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return !await _context.Users
            .AnyAsync(u => u.Id != userId &&
                          u.Email.ToLower() == email.ToLower(),
                      cancellationToken);
    }

    /// <summary>
    /// Checks if a phone number is unique excluding a specific user (for updates).
    /// </summary>
    public async Task<bool> IsPhoneUniqueForUpdateAsync(
        string phone,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return !await _context.Users
            .AnyAsync(u => u.Id != userId &&
                          u.Phone == phone,
                      cancellationToken);
    }
}