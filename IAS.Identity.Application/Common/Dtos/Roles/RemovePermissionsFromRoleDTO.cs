namespace IAS.Identity.Application.Common.Dtos.Roles;

public class RemovePermissionsFromRoleRequest
{
    public List<string> Permissions { get; set; } = new List<string>();
}