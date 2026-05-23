namespace IAS.Identity.Application.Common.Dtos.Users;

/// <summary>
/// Data transfer object for changing a user's active status
/// </summary>
/// <remarks>
/// This DTO is used to activate or deactivate user accounts without modifying other user information
/// </remarks>
public class ChangeUserStatusDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the user whose status will be changed
    /// </summary>
    /// <remarks>
    /// This field is required to identify which user to activate or deactivate
    /// </remarks>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the new active status of the user
    /// </summary>
    /// <remarks>
    /// Set to true to activate the user account, false to deactivate it.
    /// Deactivated users cannot log in to the system.
    /// </remarks>
    /// <example>true</example>
    public bool IsActive { get; set; }
}