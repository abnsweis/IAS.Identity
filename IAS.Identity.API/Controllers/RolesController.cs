using IAS.Identity.API.Infrastructure.Filters;
using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Identity.API.Controllers;

/// <summary>
/// Handles Role management operations:
/// Create, Update, Delete, Assign Permissions, and Query Roles.
/// </summary>
/// <remarks>
/// This controller provides endpoints for managing roles and their permissions.
/// All endpoints require SuperAdmin role authorization.
/// <list type="bullet">
/// <item><description>Retrieve paginated list of roles</description></item>
/// <item><description>Get role details by ID</description></item>
/// <item><description>Create new roles</description></item>
/// <item><description>Update existing roles</description></item>
/// <item><description>Delete roles</description></item>
/// <item><description>Assign permissions to roles</description></item>
/// <item><description>Remove permissions from roles</description></item>
/// </list>
/// </remarks>
[Route("api/[controller]")]
[Authorize(Roles = Roles.SuperAdmin)]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RolesController"/> class.
    /// </summary>
    /// <param name="roleService">
    /// The role service responsible for role management operations.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="roleService"/> is null.
    /// </exception>
    public RolesController(IRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    /// <summary>
    /// Retrieves a paginated list of all roles in the system.
    /// </summary>
    /// <param name="page">The page number (starting from 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10).</param>
    /// <returns>
    /// A paginated list of roles containing role details.
    /// </returns>
    /// <response code="200">Returns the paginated list of roles.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to view roles.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPaginatedListRoles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var paginatedRoles = await _roleService.GetPaginatedListRoles(page, pageSize);

        return Ok(paginatedRoles);
    }

    /// <summary>
    /// Retrieves detailed information about a specific role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <returns>
    /// The requested role details.
    /// </returns>
    /// <response code="200">Returns the requested role.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to view roles.</response>
    /// <response code="404">Role with the specified ID was not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetRoleById([FromRoute] Guid id)
    {
        var role = await _roleService.GetRoleById(id);

        return Ok(role);
    }

    /// <summary>
    /// Retrieves a paginated list of permissions assigned to a specific role.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="page">The page number (starting from 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10).</param>
    /// <returns>
    /// A paginated list of permissions assigned to the role.
    /// </returns>
    /// <response code="200">Returns the paginated list of permissions.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to view role permissions.</response>
    /// <response code="404">Role with the specified ID was not found.</response>
    [HttpGet("{roleId}/permissions")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaginatedListPermissionsByRoleId(
        [FromRoute] Guid roleId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var permissions = await _roleService
            .GetPaginatedListPermissionsByRoleId(roleId, page, pageSize);

        return Ok(permissions);
    }

    /// <summary>
    /// Creates a new role in the system.
    /// </summary>
    /// <param name="create">DTO containing the role information required for creation.</param>
    /// <returns>
    /// Returns the created role details.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Roles
    ///     {
    ///         "name": "Manager",
    ///         "description": "Manages team operations",
    ///         "permissions": ["permission-id-1", "permission-id-2"]
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Role successfully created.</response>
    /// <response code="400">Invalid role data provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to create roles.</response>
    /// <response code="409">Role with the same name already exists.</response>
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<CreateRoleDTO>))]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDTO create)
    {
        var role = await _roleService.CreateRole(create);

        return Ok(role);
    }

    /// <summary>
    /// Assigns one or more permissions to a specific role.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="request">DTO containing the list of permission IDs to assign.</param>
    /// <returns>
    /// Returns 204 No Content if the assignment succeeds.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Roles/{roleId}/permissions
    ///     {
    ///         "permissionIds": ["permission-id-1", "permission-id-2"]
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Permissions successfully assigned.</response>
    /// <response code="400">Invalid permission assignment request.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to assign permissions.</response>
    /// <response code="404">Role or permission not found.</response>
    [HttpPost("{roleId}/permissions")]
    [ServiceFilter(typeof(ValidationFilter<AssignPermissionsToRoleRequest>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignPermissionsToRole(
        [FromRoute] Guid roleId,
        [FromBody] AssignPermissionsToRoleRequest request)
    {
        await _roleService.AssignPermissionsToRole(roleId, request);

        return NoContent();
    }

    /// <summary>
    /// Updates an existing role's information.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role to update.</param>
    /// <param name="update">DTO containing the updated role information.</param>
    /// <returns>
    /// Returns 204 No Content if the update succeeds.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/Roles/{roleId}
    ///     {
    ///         "name": "Senior Manager",
    ///         "description": "Manages team operations and strategy"
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Role successfully updated.</response>
    /// <response code="400">Invalid role data provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to update roles.</response>
    /// <response code="404">Role with the specified ID was not found.</response>
    [HttpPut("{roleId}")]
    [ServiceFilter(typeof(ValidationFilter<UpdateRoleDTO>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRole(
        [FromRoute] Guid roleId,
        [FromBody] UpdateRoleDTO update)
    {
        await _roleService.UpdateRole(roleId, update);

        return NoContent();
    }

    /// <summary>
    /// Permanently deletes a role from the system.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role to delete.</param>
    /// <returns>
    /// Returns 204 No Content if deletion succeeds.
    /// </returns>
    /// <response code="204">Role successfully deleted.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to delete roles.</response>
    /// <response code="404">Role with the specified ID was not found.</response>
    [HttpDelete("{roleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRole([FromRoute] Guid roleId)
    {
        await _roleService.DeleteRole(roleId);

        return NoContent();
    }

    /// <summary>
    /// Removes one or more permissions from a role.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="request">DTO containing the list of permission IDs to remove.</param>
    /// <returns>
    /// Returns 204 No Content if removal succeeds.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/Roles/{roleId}/permissions
    ///     {
    ///         "permissionIds": ["permission-id-1", "permission-id-2"]
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Permissions successfully removed.</response>
    /// <response code="400">Invalid permission removal request.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to remove permissions.</response>
    /// <response code="404">Role or permission not found.</response>
    [HttpDelete("{roleId}/permissions")]
    [ServiceFilter(typeof(ValidationFilter<RemovePermissionsFromRoleRequest>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePermissionsFromRole(
        [FromRoute] Guid roleId,
        [FromBody] RemovePermissionsFromRoleRequest request)
    {
        await _roleService.RemovePermussionsFromRole(roleId, request);

        return NoContent();
    }
}