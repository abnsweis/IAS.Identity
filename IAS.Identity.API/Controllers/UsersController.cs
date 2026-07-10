using IAS.Identity.API.Infrastructure;
using IAS.Identity.API.Infrastructure.Filters;
using IAS.Identity.Application.Common.Dtos.Users;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IAS.Identity.API.Controllers;

/// <summary>
/// Provides endpoints for managing users,
/// including CRUD operations, status management,
/// and role assignments.
/// </summary>
/// <remarks>
/// This controller handles all user-related operations including:
/// <list type="bullet">
/// <item><description>Retrieving paginated user lists</description></item>
/// <item><description>Creating, updating, and deleting users</description></item>
/// <item><description>Activating/deactivating user accounts</description></item>
/// <item><description>Managing user roles and permissions</description></item>
/// </list>
/// </remarks>
/// <example>
/// GET: api/Users
/// POST: api/Users
/// PUT: api/Users/{id}
/// DELETE: api/Users/{id}
/// </example>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="userService">
    /// Service responsible for user management operations.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="userService"/> is null.
    /// </exception>
    public UsersController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    /// <summary>
    /// Retrieves a paginated list of users.
    /// </summary>
    /// <param name="page">The page number (starting from 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10).</param>
    /// <returns>
    /// A paginated list of users containing user details.
    /// </returns>
    /// <response code="200">Returns the paginated list of users.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to view users.</response>
    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPaginatedListUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var paginatedUsers =
            await _userService.GetPaginatedListUsersAsync(page, pageSize);

        return Ok(paginatedUsers);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="Id">The unique identifier of the user.</param>
    /// <returns>
    /// The requested user details.
    /// </returns>
    /// <response code="200">Returns the requested user.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User with the specified ID was not found.</response>
    [HttpGet("{Id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid Id)
    {
        var user = await _userService.GetUserById(Id);

        return Ok(user);
    }

    /// <summary>
    /// Retrieves all roles assigned to a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// A list of roles assigned to the user.
    /// </returns>
    /// <response code="200">Returns the user's roles.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User with the specified ID was not found.</response>
    [HttpGet("{userId}/roles")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRoles([FromRoute] string? userId)
    {
        var roles = await _userService.GetUserRoles(userId);
        return Ok(roles);
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="createUser">
    /// DTO containing the user information required for creation.
    /// </param>
    /// <returns>
    /// Returns the created user with a 201 Created response.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Users
    ///     {
    ///         "name": "John Doe",
    ///         "username": "johndoe",
    ///         "email": "john.doe@example.com",
    ///         "password": "P@ssw0rd123!",
    ///         "phone": "+1234567890",
    ///         "roles": ["role-id-1", "role-id-2"]
    ///     }
    ///
    /// </remarks>
    /// <response code="201">User successfully created.</response>
    /// <response code="400">Invalid user data provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to create users.</response>
    /// <response code="409">User with the same email or username already exists.</response>
    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    [ServiceFilter(typeof(ValidationFilter<CreateUserDto>))]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUser)
    {
        var createdUser = await _userService.CreateUserAsync(createUser);

        return CreatedAtAction(
            nameof(GetUserById),
            new { Id = createdUser.Id },
            createdUser);
    }

    /// <summary>
    /// Assigns one or more roles to a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="request">
    /// DTO containing the list of role IDs to assign.
    /// </param>
    /// <returns>
    /// Returns 204 No Content if the assignment succeeds.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Users/{userId}/roles/assign
    ///     {
    ///         "roleIds": ["role-id-1", "role-id-2"]
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Roles successfully assigned.</response>
    /// <response code="400">Invalid role assignment request.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to assign roles.</response>
    /// <response code="404">User or role not found.</response>
    [HttpPost("{userId}/roles/assign")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    [ServiceFilter(typeof(ValidationFilter<AssignRolesToUserRequest>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRoles(
        [FromRoute] Guid userId,
        [FromBody] AssignRolesToUserRequest request)
    {
        await _userService.AssignRolesAsync(userId, request);

        return NoContent();
    }

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="Id">The unique identifier of the user to update.</param>
    /// <param name="updateUser">
    /// DTO containing the updated user information.
    /// </param>
    /// <returns>
    /// Returns 204 No Content if the update succeeds.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/Users/{Id}
    ///     {
    ///         "name": "John Updated",
    ///         "username": "johnupdated",
    ///         "email": "john.updated@example.com",
    ///         "phone": "+1234567890"
    ///     }
    ///
    /// </remarks>
    /// <response code="204">User successfully updated.</response>
    /// <response code="400">Invalid user data provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to update users.</response>
    /// <response code="404">User with the specified ID was not found.</response>
    [HttpPut("{Id}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    [ServiceFilter(typeof(ValidationFilter<UpdateUserDto>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(
        [FromRoute][Required] string Id,
        [FromBody] UpdateUserDto updateUser)
    {
        await _userService.UpdateUser(Id, updateUser);

        return NoContent();
    }

    /// <summary>
    /// Activates a user account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// Returns 204 No Content if activation succeeds.
    /// </returns>
    /// <response code="204">User successfully activated.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to activate users.</response>
    /// <response code="404">User with the specified ID was not found.</response>
    [HttpPost("{userId}/activate")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser([FromRoute] Guid userId)
    {
        await _userService.ActivateUser(userId);
        return NoContent();
    }

    /// <summary>
    /// Deactivates a user account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// Returns 204 No Content if deactivation succeeds.
    /// </returns>
    /// <response code="204">User successfully deactivated.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to deactivate users.</response>
    /// <response code="404">User with the specified ID was not found.</response>
    [HttpPost("{userId}/deactivate")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser([FromRoute] Guid userId)
    {
        await _userService.DeactivateUser(userId);
        return NoContent();
    }

    /// <summary>
    /// Removes one or more roles from a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="dto">
    /// DTO containing the list of role IDs to remove.
    /// </param>
    /// <returns>
    /// Returns 204 No Content if removal succeeds.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/Users/{userId}/roles/remove
    ///     {
    ///         "roleIds": ["role-id-1", "role-id-2"]
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Roles successfully removed.</response>
    /// <response code="400">Invalid role removal request.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to remove roles.</response>
    /// <response code="404">User or role not found.</response>
    [HttpDelete("{userId}/roles/remove")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.SuperAdmin}")]
    [ServiceFilter(typeof(ValidationFilter<RemoveRolesFromUserRequest>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRoles(
        [FromRoute] Guid userId,
        [FromBody] RemoveRolesFromUserRequest dto)
    {
        await _userService.RemoveRolesAsync(userId, dto);
        return NoContent();
    }

    /// <summary>
    /// Checks if a username is available.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>
    /// Returns true if the username is available, false otherwise.
    /// </returns>
    /// <response code="200">Returns availability status.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("check-username")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckUsernameAvailability(
        [FromQuery] string username)
    {
        var isAvailable = await _userService.IsUsernameAvailableAsync(username);
        return Ok(new { IsAvailable = isAvailable });
    }

    /// <summary>
    /// Checks if an email is available.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <returns>
    /// Returns true if the email is available, false otherwise.
    /// </returns>
    /// <response code="200">Returns availability status.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("check-email")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckEmailAvailability(
        [FromQuery] string email)
    {
        var isAvailable = await _userService.IsEmailAvailableAsync(email);
        return Ok(new { IsAvailable = isAvailable });
    }
}