using IAS.Identity.Domain.Common.Constants;
using IAS.Identity.Domain.Common.Exceptions.Roles;
using IAS.Identity.Domain.Common.Exceptions.Users;

namespace IAS.Identity.Domain.Entities;

public class User : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? SecurityStamp { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public void ValidateLogin()
    {
        if (!IsActive)
        {
            throw new UserInactiveException(Id);
        }
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void AssignRole(Role role)
    {
        if (this.UserRoles.Any(r => r.RoleId == role.Id))
        {
            throw new RoleAlreadyAssignedException(this.Username, role.Name);
        }

        this.UserRoles.Add(new UserRole
        {
            UserId = this.Id,
            RoleId = role.Id
        });
    }
}