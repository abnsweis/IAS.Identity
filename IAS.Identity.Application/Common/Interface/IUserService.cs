using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Dtos.Users;
using IAS.Identity.Application.Common.Models;

namespace IAS.Identity.Application.Common.Interface;

/// <summary>
/// Provides user management operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a paginated list of users.
    /// </summary>
    Task<PaginatedList<UserDto>> GetPaginatedListUsersAsync(
        int pageNumber,
        int pageSize);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    Task UpdateUser(string userId, UpdateUserDto updateUserDto);

    /// <summary>
    /// Activates a user by identifier.
    /// </summary>
    Task ActivateUser(Guid userId);

    /// <summary>
    /// Deactivates a user by identifier.
    /// </summary>
    Task DeactivateUser(Guid userId);

    /// <summary>
    /// Retrieves a user by identifier.
    /// </summary>
    Task<UserDto> GetUserById(Guid userId);

    Task RemoveRolesAsync(Guid userId, RemoveRolesFromUserRequest request);

    Task AssignRolesAsync(Guid userId, AssignRolesToUserRequest request);

    Task<List<RoleLookUpDto>> GetUserRoles(string? userId);

    Task<bool> IsEmailAvailableAsync(string email);

    Task<bool> IsUsernameAvailableAsync(string username);
}