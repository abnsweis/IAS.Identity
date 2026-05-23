namespace IAS.Identity.Application.Common.Dtos.Users;

/// <summary>
/// Data transfer object for updating an existing user
/// </summary>
/// <remarks>
/// This DTO is used in PUT/PATCH requests to update user information.
/// All fields are optional to support partial updates.
/// </remarks>
public class UpdateUserDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the user to update
    /// </summary>
    /// <remarks>
    /// This field is required to identify which user to update
    /// </remarks>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the updated full name of the user (optional)
    /// </summary>
    /// <remarks>
    /// If not provided, the existing name will be kept unchanged
    /// </remarks>
    /// <example>Johnathan Doe</example>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the updated email address of the user (optional)
    /// </summary>
    /// <remarks>
    /// If provided, the email must be unique and in valid format
    /// </remarks>
    /// <example>ibrahimalsweis@ias.com</example>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the updated username (optional)
    /// </summary>
    /// <remarks>
    /// If provided, the username must be unique in the system
    /// </remarks>
    /// <example>sweis</example>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the updated phone number (optional)
    /// </summary>
    /// <remarks>
    /// If not provided, the existing phone number will be kept unchanged.
    /// Can be set to null to remove the phone number.
    /// </remarks>
    /// <example>+1987654321</example>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the list of role IDs to replace existing role assignments
    /// </summary>
    /// <remarks>
    /// If provided, this list will completely replace the user's existing role assignments.
    /// If empty list is provided, the user will be removed from all roles.
    /// If null, existing role assignments will be kept unchanged.
    /// </remarks>
    public List<Guid> Roles { get; set; } = new List<Guid>();
}