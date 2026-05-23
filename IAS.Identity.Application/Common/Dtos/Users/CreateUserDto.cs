namespace IAS.Identity.Application.Common.Dtos.Users;

/// <summary>
/// Data transfer object for creating a new user
/// </summary>
/// <remarks>
/// This DTO is used in POST requests to create new users. All required fields must be provided.
/// </remarks>
public class CreateUserDto
{
    /// <summary>
    /// Gets or sets the full name of the user
    /// </summary>
    /// <remarks>
    /// This field is required and cannot be null or empty
    /// </remarks>
    /// <example>Ibrahim AL Sweis</example>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the email address of the user
    /// </summary>
    /// <remarks>
    /// This field is required and must be a valid email format. Email addresses must be unique in the system.
    /// </remarks>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the username for authentication
    /// </summary>
    /// <remarks>
    /// This field is required and must be unique in the system. Usernames are case-sensitive.
    /// </remarks>
    /// <example>ibrahimalsweis@IAS.com</example>
    public string Username { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password for the user account
    /// </summary>
    /// <remarks>
    /// This field is required and will be hashed before storage.
    /// Password should meet complexity requirements (minimum length, uppercase, lowercase, number, special character).
    /// </remarks>
    /// <example>P@ssw0rd123!</example>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Gets or sets the phone number of the user (optional)
    /// </summary>
    /// <remarks>
    /// This field is optional and can be null
    /// </remarks>
    /// <example>+1234567890</example>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the list of role IDs to assign to the user
    /// </summary>
    /// <remarks>
    /// This collection contains role GUIDs. User will be assigned to all roles in this list.
    /// If empty or null, the user will be assigned to the default "User" role.
    /// </remarks>
    public List<Guid> Roles { get; set; } = new List<Guid>();
}