using FluentValidation;
using IAS.Identity.Application.Common;
using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Dtos.Users;
using IAS.Identity.Application.Common.Exceptions;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Application.Common.Models;
using IAS.Identity.Domain.Entities;
using IAS.Identity.Domain.Enums;
using IAS.Identity.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using static IAS.Identity.Application.Common.ErrorCodes;

namespace IAS.Identity.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IUserUniquenessValidator _userUniquenessValidator;

    public UserService(ApplicationDbContext context, IPasswordHasherService passwordHasher, IUserUniquenessValidator userUniquenessValidator)
    {
        this._context = context;
        this._passwordHasher = passwordHasher;
        this._userUniquenessValidator = userUniquenessValidator;
    }

    /// <summary>
    /// Creates a new user with the provided information and assigned roles.
    /// </summary>
    /// <param name="createUserDto">
    /// DTO containing user information such as name, email, username,
    /// password, phone number, and assigned roles.
    /// </param>
    /// <returns>
    /// Returns the created user as a <see cref="UserDto"/>.
    /// </returns>
    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        await _ValidateCreateUserData(createUserDto);
        // Generate a new unique identifier for the user
        var userId = Guid.NewGuid();

        #region Local function to create user roles

        /// <summary>
        /// Creates the user-role relationships based on the provided role IDs.
        /// </summary>
        /// <returns>A list of <see cref="UserRole"/> entities.</returns>
        List<UserRole> _CreateUserRoles()
        {
            var userRoles = new List<UserRole>();

            // Convert role IDs from string to Guid
            var roleGuids = createUserDto.Roles
                .Select(Guid.Parse)
                .ToList();

            // Retrieve matching roles from database
            var roles = _context.Roles
                .Where(r => roleGuids.Contains(r.Id))
                .ToList();

            // Create relationship entries between user and roles
            foreach (var role in roles)
            {
                userRoles.Add(new UserRole
                {
                    RoleId = role.Id,
                    UserId = userId,
                    Role = role
                });
            }

            return userRoles;
        }

        #endregion Local function to create user roles

        // Create the user entity
        var newUser = new User
        {
            Id = userId,
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            Username = createUserDto.Username,
            Phone = createUserDto.Phone,

            // Hash the password before storing it
            PasswordHash = _passwordHasher.Hash(createUserDto.Password),

            // Assign roles to the user
            UserRoles = _CreateUserRoles()
        };

        // Add user to database
        await _context.Users.AddAsync(newUser);

        // Persist changes
        await _context.SaveChangesAsync();

        // Return mapped DTO
        return _MapToUserDto(newUser);
    }

    public async Task<PaginatedList<UserDto>> GetPaginatedListUsersAsync(
        int pageNumber, int pageSize)
    {
        // 1️⃣ Build the query — nothing is executed yet (IQueryable)
        var query = _context.Users
            .Include(u => u.UserRoles)
            .AsNoTracking()                         // read-only = faster
            .OrderBy(u => u.Id)                     // ORDER BY required for pagination
            .Where(u => !u.IsDeleted)
            .Select(u => new UserDto               // projection pushed into SQL
            {
                Id = u.Id,
                Name = u.Name,
                Username = u.Username,
                Email = u.Email,
                Phone = u.Phone,
                Status = u.Status.ToString(),
                Roles = u.UserRoles
                            .Select(r => new RoleLookUpDto { Name = r.Role!.Name, RoleId = r.RoleId })
                            .ToList()
            });

        return await PaginatedList<UserDto>.CreateAsync(query, pageNumber, pageSize);
    }

    public async Task<UserDto> GetUserById(Guid userId)
    {
        var user = await _context.Users
            .Where(u => !u.IsDeleted)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            throw new NotFoundException($"User with id '{userId}' was not found.", ErrorCodes.Users.USER_NOT_FOUND);

        return _MapToUserDto(user);
    }

    /// <summary>
    /// Updates an existing user with the provided information.
    /// </summary>
    /// <param name="userId">The user identifier from the request URL.</param>
    /// <param name="updateUserDto">The updated user data.</param>
    /// <exception cref="BadRequestException">
    /// Thrown when the user ID in the URL does not match the user ID in the request body.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the specified user does not exist or is deleted.
    /// </exception>
    public async Task UpdateUser(string userId, UpdateUserDto updateUserDto)
    {
        // Ensure the route ID matches the request body ID
        if (userId != updateUserDto.Id)
            throw new BadRequestException(
                $"User id in the URL '{userId}' does not match user id in the body '{updateUserDto.Id}'.",
                ErrorCodes.Users.USER_ID_MISMATCH);

        // Retrieve the user including related roles, excluding soft-deleted users
        var user = _context.Users
            .Where(u => !u.IsDeleted)
            .Include(u => u.UserRoles)
            .FirstOrDefault(u => u.Id == Guid.Parse(updateUserDto.Id));

        // Validate that the user exists
        if (user is null)
            throw new NotFoundException(
                $"User with id '{updateUserDto.Id}' was not found.",
                ErrorCodes.Users.USER_NOT_FOUND);

        // Map updated fields from DTO to entity
        updateUserDto.Adapt(user);

        // Mark entity as updated and persist changes
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Assigns a set of roles to a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to assign roles to.</param>
    /// <param name="request">The DTO containing the list of role IDs to assign to the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="NotFoundException">Thrown when a user with the specified ID does not exist or is marked as deleted.</exception>
    public async Task AssignRolesAsync(Guid userId, AssignRolesToUserRequest request)
    {
        var roleIds = request.Roles.Select(Guid.Parse).ToList();

        var user = await _context.Users
            .Where(u => !u.IsDeleted)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException(
                $"User with id '{userId}' was not found.",
                ErrorCodes.Users.USER_NOT_FOUND);

        var roles = await _context.Roles
            .Where(r => roleIds.Contains(r.Id) && !r.IsDeleted)
            .ToListAsync();

        var notFoundIds = roleIds.Except(roles.Select(r => r.Id)).ToList();
        if (notFoundIds.Count > 0)
            throw new NotFoundException(
                $"Roles not found: {string.Join(", ", notFoundIds)}",
                ErrorCodes.Roles.ROLE_NOT_FOUND);

        foreach (var role in roles)
            user.AssignRole(role);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Maps a User entity to a UserDto object.
    /// </summary>
    /// <param name="user">The User entity to be mapped.</param>
    /// <returns>A UserDto containing the mapped user data including associated roles.</returns>
    /// <remarks>
    /// This method transforms the User entity along with its related UserRoles and Role data
    /// into a flattened DTO suitable for API responses or client consumption.
    /// </remarks>
    private UserDto _MapToUserDto(User user)
    {
        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Username = user.Username,
            Phone = user.Phone,
            Status = user.Status.ToString(),
            Roles = user.UserRoles.Select(ur => new RoleLookUpDto { RoleId = ur.RoleId, Name = ur.Role!.Name }).ToList()
        };
        return userDto;
    }

    public async Task RemoveRolesAsync(Guid userId, RemoveRolesFromUserRequest removeRoles)
    {
        var roleIds = removeRoles.Roles.Select(Guid.Parse).ToList();

        var user = await _context.Users
            .Where(u => !u.IsDeleted)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException(
                $"User with id '{userId}' was not found.",
                ErrorCodes.Users.USER_NOT_FOUND);

        var roles = await _context.Roles
            .Where(r => roleIds.Contains(r.Id) && !r.IsDeleted)
            .ToListAsync();

        var notFoundIds = roleIds.Except(roles.Select(r => r.Id)).ToList();
        if (notFoundIds.Count > 0)
            throw new NotFoundException(
                $"Roles not found: {string.Join(", ", notFoundIds)}",
                ErrorCodes.Roles.ROLE_NOT_FOUND);

        foreach (var role in roles)
            user.RemoveRole(role);

        await _context.SaveChangesAsync();
    }

    public async Task<List<RoleLookUpDto>> GetUserRoles(string? userId)
    {
        if (!Guid.TryParse(userId, out var guid))
            throw new BadRequestException($"'{userId}' is not a valid ID.", ErrorCodes.Users.INVALID_USER_ID);

        var user = await _context.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == guid);

        if (user == null)
            throw new NotFoundException($"User with id '{userId}' was not found.", ErrorCodes.Users.USER_NOT_FOUND);
        if (!user.UserRoles.Any())
            return new List<RoleLookUpDto>();

        var roles = user.UserRoles.Select(ur => new RoleLookUpDto
        {
            RoleId = ur.RoleId,
            Name = ur.Role!.Name
        }).ToList();

        return roles;
    }

    public async Task ActivateUser(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        if (user == null)
            throw new NotFoundException($"User with id '{userId}' was not found.", ErrorCodes.Users.USER_NOT_FOUND);

        user.ChangeStatus(enUserStatus.Active);
        await _context.SaveChangesAsync();
    }

    public async Task DeactivateUser(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        if (user == null)
            throw new NotFoundException($"User with id '{userId}' was not found.", ErrorCodes.Users.USER_NOT_FOUND);

        user.ChangeStatus(enUserStatus.Inactive);
        await _context.SaveChangesAsync();
    }

    private async Task _ValidateCreateUserData(CreateUserDto userDto)
    {
        var validationErrors = await _userUniquenessValidator.ValidateUniquenessAsync(userDto);
        if (validationErrors.Any())
            throw new BadRequestException(
                validationErrors.ToDictionary(
                    x => x.Key,
                    x => new[] { x.Value }
                )
            );
    }

    public async Task<bool> IsEmailAvailableAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsUsernameAvailableAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }
}