using IAS.Identity.Domain.Common.Constants;
using IAS.Identity.Domain.Common.Exceptions.Roles;
using IAS.Identity.Domain.Common.Exceptions.Users;
using IAS.Identity.Domain.Enums;

namespace IAS.Identity.Domain.Entities;

public class User : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? SecurityStamp { get; set; }
    public enUserStatus Status { get; set; } = enUserStatus.Active;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public void ValidateLogin()
    {
        if (this.Status == enUserStatus.Inactive)
        {
            throw new UserInactiveException(Id);
        }
    }

    public void ChangeStatus(enUserStatus status)
    {
        Status = status;
    }

    public void AssignRole(Role role)
    {
        if (this.UserRoles.Any(r => r.RoleId == role.Id)) return;
         
        this.UserRoles.Add(new UserRole
        {
            UserId = this.Id,
            RoleId = role.Id
        });
    }

    public void RemoveRole(Role role)
    {
        var userRole = this.UserRoles.FirstOrDefault(r => r.RoleId == role.Id);

        if (userRole == null) return;  

        this.UserRoles.Remove(userRole);
    }
}