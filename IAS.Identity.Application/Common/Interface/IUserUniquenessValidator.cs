namespace IAS.Identity.Application.Common.Interface;

using global::IAS.Identity.Application.Common.Dtos.Users;

/// <summary>
/// Service for validating user uniqueness in the system.
/// </summary>
public interface IUserUniquenessValidator
{
    /// <summary>
    /// Validates if the user's unique fields (name, username, email, phone) are available.
    /// </summary>
    /// <param name="dto">The user DTO to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A dictionary of field names and their corresponding error messages if validation fails.
    /// Returns empty dictionary if all fields are unique.
    /// </returns>
    Task<Dictionary<string, string>> ValidateUniquenessAsync(
        CreateUserDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a name is unique in the system.
    /// </summary>
    Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a username is unique in the system.
    /// </summary>
    Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an email is unique in the system.
    /// </summary>
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a phone number is unique in the system.
    /// </summary>
    Task<bool> IsPhoneUniqueAsync(string phone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a name is unique in the system excluding a specific user (for updates).
    /// </summary>
    Task<bool> IsNameUniqueForUpdateAsync(string name, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a username is unique in the system excluding a specific user (for updates).
    /// </summary>
    Task<bool> IsUsernameUniqueForUpdateAsync(string username, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an email is unique in the system excluding a specific user (for updates).
    /// </summary>
    Task<bool> IsEmailUniqueForUpdateAsync(string email, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a phone number is unique in the system excluding a specific user (for updates).
    /// </summary>
    Task<bool> IsPhoneUniqueForUpdateAsync(string phone, Guid userId, CancellationToken cancellationToken = default);
}