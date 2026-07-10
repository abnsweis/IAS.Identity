using IAS.Identity.Domain.Enums;

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
    /// Gets or sets the new active status of the user
    /// </summary>
    /// <remarks>
    /// Set to true to activate the user account, false to deactivate it.
    /// Deactivated users cannot log in to the system.
    /// </remarks>
    /// <example>true</example>
    public enUserStatus Status { get; set; }
}