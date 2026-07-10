using IAS.Identity.Application.Common;
using IAS.Identity.Application.Common.Dtos.Auth;
using IAS.Identity.Application.Common.Dtos.Users;
using IAS.Identity.Application.Common.Exceptions;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Domain.Common.Constants;
using IAS.Identity.Domain.Entities;
using IAS.Identity.Infrastructure.Data;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace IAS.Identity.Infrastructure.Services;

internal class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public AuthService(ApplicationDbContext context,
        IPasswordHasherService passwordHasher,
        ITokenService tokenService,
        ICurrentUser currentUser,
        IUserService userService,
        IRoleService roleService)
    {
        this._context = context;
        this._passwordHasher = passwordHasher;
        this._tokenService = tokenService;
        this._currentUser = currentUser;
        this._userService = userService;
        this._roleService = roleService;
    }

    public async Task<RegisterResponse> Register(RegisterUserDto registerUserDto)
    {
        // Generate a new unique identifier for the user
        var userId = Guid.NewGuid();

        // Create the user entity
        var newUser = _CreateUser(registerUserDto);

        // Add user to database
        await _context.Users.AddAsync(newUser);

        // Assign 'user' role
        await _AssignUserRoleForRegisterdUser(newUser);

        // Persist changes
        await _context.SaveChangesAsync();

        // Retrieve the created user from the database to ensure we have the latest data
        var createdUser = await _GetUser(newUser.Username);

        // Generate JWT token for the created user
        var token = _tokenService.GenerateToken(createdUser);

        // Generate a refresh token for the created user
        var refreshToken = await _CreateRefreshToken(createdUser.Id);

        // Return the response containing the JWT token, refresh token, and user ID
        var response = new RegisterResponse
        {
            Token = token,
            RefrshToken = refreshToken,
            UserId = createdUser.Id.ToString(),
        };

        return response;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _GetUser(loginRequest.Username);

        var isValid = IsValidCredentialsAsync(loginRequest, user);

        if (!isValid)
            throw new BadRequestException("Invalid username or password.", ErrorCodes.Auth.INVALID_CREDENTIALS);

        var token = _tokenService.GenerateToken(user!);

        var refreshToken = await _CreateRefreshToken(user.Id);

        return new LoginResponse
        {
            Token = token,
            RefrshToken = refreshToken,
            UserId = user.Id.ToString(),
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto refreshTokenRequest)
    {
        RefreshToken? refreshToken = _context.RefreshTokens
            .Include(r => r.User).FirstOrDefault(rt => rt.Token == refreshTokenRequest.RefreshToken);

        if (refreshToken is null || refreshToken.ExpiresOnUTC < DateTimeOffset.UtcNow)
        {
            throw new BadRequestException("The refresh token has expired", ErrorCodes.Auth.REFRESH_TOKEN_EXPIRED);
        }

        var newToken = _tokenService.GenerateToken(refreshToken.User);

        refreshToken.Token = _tokenService.GenerateRefreshToken();
        refreshToken.ExpiresOnUTC = DateTimeOffset.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            Token = newToken,
            RefrshToken = refreshToken.Token,
            UserId = refreshToken.UserId.ToString(),
        };
    }

    public Task LogoutAsync(string refreshToken)
    {
        var userId = _currentUser.UserID;
        if (userId is null || Guid.TryParse(userId, out var Id))
            throw new BadRequestException("User is not authenticated", ErrorCodes.Auth.USER_NOT_AUTHENTICATED);

        return _context.RefreshTokens.Where(r => r.UserId == Id && r.Token == refreshToken).ExecuteDeleteAsync();
    }

    private bool IsValidCredentialsAsync(LoginRequestDto loginRequest, User user)
    {
        return _passwordHasher.Verify(loginRequest.Password, user.PasswordHash);
    }

    private async Task<string> _CreateRefreshToken(Guid userId)
    {
        var token = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = userId,
            ExpiresOnUTC = DateTimeOffset.UtcNow.AddDays(7) // Set the expiration time for the refresh token
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return token;
    }

    private async Task<User> _GetUser(string username)
    {
        var user = await _context.Users.Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Username == username);
        if (user is null)
            throw new BadRequestException("Invalid username or password.", ErrorCodes.Auth.INVALID_CREDENTIALS);
        return user;
    }

    public async Task RevokeTokens(string userId)
    {
        if (!Guid.TryParse(userId, out Guid userIdGuid))
        {
            throw new BadRequestException("Invalid user id guid", ErrorCodes.General.INVALID_GUID);
        }
        await _context.RefreshTokens.Where(r => r.UserId == userIdGuid).ExecuteDeleteAsync();
    }

    private User _CreateUser(RegisterUserDto registerUserDto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = registerUserDto.Name,
            Email = registerUserDto.Email,
            Username = registerUserDto.Username,
            Phone = registerUserDto.Phone,
            PasswordHash = _passwordHasher.Hash(registerUserDto.Password)
        };
        return user;
    }

    private async Task _AssignUserRoleForRegisterdUser(User registerdUser)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == Roles.User);
        if (role is null)
            throw new InternalServerErrorException($"Role '{Roles.User}' does not exist");
        registerdUser.AssignRole(role);
    }

    public Task<UserDto> GetMe()
    {
        var userId = _currentUser.UserID;
        if (userId is null || !Guid.TryParse(userId, out var Id))
            throw new UnauthorizedException("User is not authenticated");

        return _userService.GetUserById(Id);
    }
}