using IAS.Identity.Application.Common.Interface;
using IAS.Identity.Domain.Common.Constants;
using System.Security.Claims;

namespace IAS.Identity.API.Infrastructure;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserID => User?.FindFirstValue(ClaimTypes.NameIdentifier);
     
    public bool IsAdmin => User?.HasClaim(ClaimTypes.Role, Roles.Admin) ?? false;

    public bool SuperAdmin => User?.HasClaim(ClaimTypes.Role, Roles.SuperAdmin) ?? false;

    public bool HasPermission(string permissionName) =>
        User?.HasClaim("Permissions",permissionName) ?? false;

    public bool HasRole(string roleName) =>
        User?.HasClaim(ClaimTypes.Role, roleName) ?? false;
}