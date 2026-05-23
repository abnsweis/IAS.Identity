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
    Task<UserDto> UpdateUser(UpdateUserDto updateUserDto);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    Task DeleteUser(Guid userId);

    /// <summary>
    /// Retrieves a user by identifier.
    /// </summary>
    Task<UserDto> GetUserById(Guid userId);

    /// <summary>
    /// Changes user activation status.
    /// </summary>
    Task<bool> Activate(Guid userId);

    Task<bool> Deactivate(Guid userId);

    Task AssignRole(Guid userId, Guid roleId);

    Task AssignRolesAsync(Guid userId, List<Guid> RoleIds);
}