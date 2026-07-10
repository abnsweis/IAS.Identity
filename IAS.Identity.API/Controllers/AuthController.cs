using IAS.Identity.API.Infrastructure.Filters;
using IAS.Identity.Application.Common.Dtos.Auth;
using IAS.Identity.Application.Common.Dtos.Users;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Domain.Common.Constants;
using IAS.Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Identity.API.Controllers;

/// <summary>
/// Handles authentication-related operations:
/// Login, Register, Token refresh, Logout, and fetching current user info.
/// </summary>
/// <remarks>
/// This controller manages user authentication and session management.
/// <list type="bullet">
/// <item><description>User login with JWT token generation</description></item>
/// <item><description>User registration with validation</description></item>
/// <item><description>Token refresh using refresh tokens</description></item>
/// <item><description>User logout with token revocation</description></item>
/// <item><description>Token revocation for specific users (Admin only)</description></item>
/// <item><description>Get current authenticated user information</description></item>
/// </list>
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">
    /// The authentication service responsible for auth operations.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="authService"/> is null.
    /// </exception>
    public AuthController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens.
    /// Also stores the refresh token in an HttpOnly secure cookie.
    /// </summary>
    /// <param name="request">The login credentials containing email/username and password.</param>
    /// <returns>
    /// Returns an access token and refresh token for authenticated user.
    /// The refresh token is also stored in a secure HTTP-only cookie.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Auth/login
    ///     {
    ///         "email": "user@example.com",
    ///         "password": "P@ssw0rd123!"
    ///     }
    ///
    /// The refresh token is automatically stored in a secure HttpOnly cookie.
    /// </remarks>
    /// <response code="200">User successfully authenticated. Returns access and refresh tokens.</response>
    /// <response code="400">Invalid login credentials.</response>
    /// <response code="401">Authentication failed.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // Validate user credentials and generate tokens
        var token = await _authService.LoginAsync(request);

        // Store refresh token in secure HttpOnly cookie
        Response.Cookies.Append(
            "refreshToken",
            token.RefrshToken,
            new CookieOptions
            {
                HttpOnly = true,              // Prevents JavaScript access (protects against XSS)
                Secure = true,                // Only sent over HTTPS
                SameSite = SameSiteMode.None, // Required for cross-site requests (SPA apps)
                Expires = DateTime.UtcNow.AddDays(7), // Cookie expiration
                Domain = null,                // Set domain if needed
                Path = "/api/Auth"            // Only sent to auth endpoints
            });

        // Return access token + refresh token info
        return Ok(token);
    }

    /// <summary>
    /// Registers a new user account in the system.
    /// </summary>
    /// <param name="registerUser">DTO containing user registration information.</param>
    /// <returns>
    /// Returns 201 Created response with the registered user information.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Auth/register
    ///     {
    ///         "name": "John Doe",
    ///         "username": "johndoe",
    ///         "email": "john.doe@example.com",
    ///         "password": "P@ssw0rd123!",
    ///         "confirmPassword": "P@ssw0rd123!",
    ///         "phone": "+1234567890"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">User successfully registered.</response>
    /// <response code="400">Invalid registration data provided.</response>
    /// <response code="409">User with the same email or username already exists.</response>
    [HttpPost("register")]
    [ServiceFilter(typeof(ValidationFilter<RegisterUserDto>))]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUser)
    {
        // Create new user account
        var result = await _authService.Register(registerUser);

        // Return 201 Created response
        return CreatedAtAction(
            nameof(UsersController.GetUserById),
            "Users",
            new { Id = result.UserId },
            result);
    }

    /// <summary>
    /// Generates a new access token using a valid refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token used to generate new access token.</param>
    /// <returns>
    /// Returns new access token and updated refresh token.
    /// </returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Auth/refresh-tokens
    ///     {
    ///         "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    ///     }
    ///
    /// </remarks>
    /// <response code="200">New tokens successfully generated.</response>
    /// <response code="400">Invalid refresh token.</response>
    /// <response code="401">Refresh token expired or revoked.</response>
    [HttpPost("refresh-tokens")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshToken)
    {
        var token = await _authService.RefreshTokenAsync(refreshToken);

        return Ok(token);
    }

    /// <summary>
    /// Logs out the current user and revokes the refresh token.
    /// </summary>
    /// <returns>
    /// Returns 204 No Content if logout succeeds.
    /// </returns>
    /// <remarks>
    /// This endpoint:
    /// 1. Retrieves refresh token from cookie
    /// 2. Revokes the token in the backend
    /// 3. Clears the refresh token cookie
    /// </remarks>
    /// <response code="204">User successfully logged out.</response>
    /// <response code="401">User is not authenticated.</response>
    [Authorize]
    [HttpDelete("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        // Retrieve refresh token from cookie
        var refreshToken = Request.Cookies["refreshToken"];

        // Revoke token in backend storage
        await _authService.LogoutAsync(refreshToken);

        // Clear the refresh token cookie
        Response.Cookies.Delete("refreshToken");

        return NoContent();
    }

    /// <summary>
    /// Revokes all tokens for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// Returns 204 No Content if revocation succeeds.
    /// </returns>
    /// <remarks>
    /// This endpoint is typically used by admin or security systems to:
    /// - Force logout a user
    /// - Revoke compromised tokens
    /// - Implement security policies
    /// </remarks>
    /// <response code="204">All tokens successfully revoked.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have permission to revoke tokens.</response>
    /// <response code="404">User with the specified ID was not found.</response>
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
    [HttpDelete("revoke-tokens/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeTokens([FromRoute] string userId)
    {
        await _authService.RevokeTokens(userId);

        return NoContent();
    }

    /// <summary>
    /// Retrieves information about the currently authenticated user.
    /// </summary>
    /// <returns>
    /// Returns the current user's profile information.
    /// </returns>
    /// <response code="200">Returns the current user's information.</response>
    /// <response code="401">User is not authenticated.</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe()
    {
        var user = await _authService.GetMe();

        return Ok(user);
    }
}