using IAS.Identity.API.Infrastructure.Filters;
using IAS.Identity.Application.Common.Dtos.Permissions;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Identity.API.Controllers;

/// <summary>
/// Handles permission management operations (CRUD).
/// Access is restricted to Admin and SuperAdmin roles.
/// </summary>
/// <remarks>
/// This controller provides endpoints for managing permissions in the system.
/// All endpoints require Admin or SuperAdmin role authorization.
/// <list type="bullet">
/// <item><description>Retrieve paginated list of permissions</description></item>
/// <item><description>Get permission details by ID</description></item>
/// <item><description>Create new permissions</description></item>
/// <item><description>Update existing permissions</description></item>
/// <item><description>Delete permissions</description></item>
/// </list>
/// </remarks>
/// <example>
/// GET: api/Permission
/// POST: api/Permission
/// PUT: api/Permission/{permissionId}
/// DELETE: api/Permission/{id}
/// </example>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionController"/> class.
    /// </summary>
    /// <param name="permissionService">
    /// The permission service responsible for permission management operations.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="permissionService"/> is null.
    /// </exception>
    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
    }

    /// <summary>
    /// Retrieves a paginated list of all permissions in the system.
    /// </summary>
    /// <param name="pageNumber">The page number (starting from 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10).</param>
    /// <returns>
    /// A paginated list of permissions containing permission details.
    /// </returns>
    /// <response code="200">Returns the paginated list of permissions.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to view permissions.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPaginatedListPermissions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var permissions = await _permissionService
            .GetPaginatedListPermissions(pageNumber, pageSize);

        return Ok(permissions);
    }

    /// <summary>
    /// Retrieves detailed information about a specific permission by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the permission.</param>
    /// <returns>
    /// The requested permission details.
    /// </returns>
    /// <response code="200">Returns the requested permission.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to view permissions.</response>
    /// <response code="404">Permission with the specified ID was not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PermissionDetailsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPermissionById([FromRoute] Guid id)
    {
        var permission = await _permissionService.GetPermissionById(id);

        return Ok(permission);
    }

    /// <summary>
    /// Creates a new permission in the system.
    /// </summary>
    /// <param name="create">DTO containing the permission information required for creation.</param>
    /// <returns>
    /// Returns the created permission details.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Permission
    ///     {
    ///         "name": "View Users",
    ///         "description": "Allows user to view list of users",
    ///         "resource": "Users",
    ///         "action": "Read"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Permission successfully created.</response>
    /// <response code="400">Invalid permission data provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to create permissions.</response>
    /// <response code="409">Permission with the same name already exists.</response>
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<CreatePermissionRequest>))]
    [ProducesResponseType(typeof(PermissionDetailsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest create)
    {
        var permission = await _permissionService.CreatePermission(create);

        return Ok(permission);
    }

    /// <summary>
    /// Permanently deletes a permission from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the permission to delete.</param>
    /// <returns>
    /// Returns 204 No Content if deletion succeeds.
    /// </returns>
    /// <response code="204">Permission successfully deleted.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to delete permissions.</response>
    /// <response code="404">Permission with the specified ID was not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePermission([FromRoute] Guid id)
    {
        await _permissionService.DeletePermissionAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Updates an existing permission's information.
    /// </summary>
    /// <param name="permissionId">The unique identifier of the permission to update.</param>
    /// <param name="update">DTO containing the updated permission information.</param>
    /// <returns>
    /// Returns 204 No Content if the update succeeds.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/Permission/{permissionId}
    ///     {
    ///         "name": "View Users and Roles",
    ///         "description": "Allows user to view list of users and their roles",
    ///         "resource": "Users",
    ///         "action": "Read"
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Permission successfully updated.</response>
    /// <response code="400">Invalid permission data provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to update permissions.</response>
    /// <response code="404">Permission with the specified ID was not found.</response>
    [HttpPut("{permissionId}")]
    [ServiceFilter(typeof(ValidationFilter<UpdatePermissionRequest>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePermission(
        [FromRoute] Guid permissionId,
        [FromBody] UpdatePermissionRequest update)
    {
        await _permissionService.UpdatePermission(permissionId, update);

        return NoContent();
    }
}