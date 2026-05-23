using IAS.Identity.Application.Common.Dtos.Roles;

namespace IAS.Identity.Application.Common.Dtos.Users;

/// <summary>
/// Data transfer object for returning user information in API responses
/// </summary>
/// <remarks>
/// This DTO is used when retrieving user details and lists, excluding sensitive information like passwords
/// </remarks>
public class UserDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the user
    /// </summary>
    /// <remarks>
    /// This is a nullable GUID that will be null for new users not yet persisted
    /// </remarks>
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the full name of the user
    /// </summary>
    /// <example>Ibrahim Al Sweis</example>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user
    /// </summary>
    /// <example>ibrahimalsweis@IAS.com</example>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the username used for authentication
    /// </summary>
    /// <example>sweis</example>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the user
    /// </summary>
    /// <example>+1234567890</example>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the list of role names assigned to the user
    /// </summary>
    /// <remarks>
    /// Contains role names like "Admin", "User" rather than role IDs
    /// </remarks>
    public List<RoleLookUpDto> Roles { get; set; } = new List<RoleLookUpDto>();

    /// <summary>
    /// Gets or sets a value indicating whether the user account is active
    /// </summary>
    /// <remarks>
    /// Active users can log in; inactive users cannot
    /// </remarks>
    public bool IsActive { get; set; }
}