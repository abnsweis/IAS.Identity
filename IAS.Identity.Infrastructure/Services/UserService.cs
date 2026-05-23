using IAS.Identity.Application.Common;
using IAS.Identity.Application.Common.Dtos.Roles;
using IAS.Identity.Application.Common.Dtos.Users;
using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Application.Common.Models;
using IAS.Identity.Domain.Entities;
using IAS.Identity.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using YourApp.Infrastructure.Exceptions;

namespace IAS.Identity.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;

    public UserService(ApplicationDbContext context, IPasswordHasherService passwordHasher)
    {
        this._context = context;
        this._passwordHasher = passwordHasher;
    }

    public async Task<bool> Deactivate(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new NotFoundException(
                "USER_NOT_FOUND",
                $"User with id '{userId}' was not found.");

        user.Deactivate();

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> Activate(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new NotFoundException($"User with id '{userId}' was not found.", ErrorCodes.Users.USER_NOT_FOUND);

        user.Activate();

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var userId = Guid.NewGuid();

        #region Local function to create user roles

        List<UserRole> _CreateUserRoles()
        {
            var userRoles = new List<UserRole>();
            var roles = _context.Roles.Where(r => createUserDto.Roles.Contains(r.Id)).ToList();
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

        var newUser = new User
        {
            Id = userId,
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            Username = createUserDto.Username,
            Phone = createUserDto.Phone,
            PasswordHash = _passwordHasher.Hash(createUserDto.Password),
            UserRoles = _CreateUserRoles()
        };

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return _MapToUserDto(newUser);
    }

    public Task DeleteUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<PaginatedList<UserDto>> GetPaginatedListUsersAsync(
        int pageNumber, int pageSize)
    {
        // 1️⃣ Build the query — nothing is executed yet (IQueryable)
        var query = _context.Users
            .Include(u => u.UserRoles)
            .AsNoTracking()                         // read-only = faster
            .OrderBy(u => u.Id)                     // ORDER BY required for pagination
            .Select(u => new UserDto               // projection pushed into SQL
            {
                Id = u.Id,
                Name = u.Name,
                Username = u.Username,
                Email = u.Email,
                Phone = u.Phone,
                IsActive = u.IsActive,
                Roles = u.UserRoles
                            .Select(r => new RoleLookUpDto { Name = r.Role!.Name, RoleId = r.RoleId })
                            .ToList()
            });

        return await PaginatedList<UserDto>.CreateAsync(query, pageNumber, pageSize);
    }

    public async Task<UserDto> GetUserById(Guid userId)
    {
        var user = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            throw new NotFoundException($"User with id '{userId}' was not found.", ErrorCodes.Users.USER_NOT_FOUND);

        return _MapToUserDto(user);
    }

    public Task<UserDto> UpdateUser(UpdateUserDto updateUserDto)
    {
        throw new NotImplementedException();
    }

    public Task AssignRole(Guid userId, Guid roleId)
    {
        throw new NotImplementedException();
    }

    public async Task AssignRolesAsync(Guid userId, List<Guid> RoleIds)
    {
        var user = _context.Users.Include(u => u.UserRoles).FirstOrDefault(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException($"User with id '{userId}' was not found.", ErrorCodes.Users.USER_NOT_FOUND);

        var roles = _context.Roles.Where(r => RoleIds.Contains(r.Id)).ToList();

        foreach (var role in roles)
        {
            user.AssignRole(role);
        }

        await _context.SaveChangesAsync();
    }

    private UserDto _MapToUserDto(User user)
    {
        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Username = user.Username,
            Phone = user.Phone,
            IsActive = user.IsActive,
            Roles = user.UserRoles.Select(ur => new RoleLookUpDto { RoleId = ur.RoleId, Name = ur.Role!.Name }).ToList()
        };
        return userDto;
    }
}