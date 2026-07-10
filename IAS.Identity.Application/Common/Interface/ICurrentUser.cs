namespace IAS.Identity.Application.Common.Interface;

public interface ICurrentUser
{
    string UserID { get; }

    bool IsAdmin { get; }
    bool SuperAdmin { get; }

    bool HasRole(string roleName);

    bool HasPermission(string permissionName);
}