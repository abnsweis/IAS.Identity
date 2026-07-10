namespace IAS.Identity.Application.Common.Dtos.Roles;

public class AssignPermissionsToRoleRequest
{
    public List<string> Permissions { get; set; } = new List<string>();
}