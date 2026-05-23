namespace IAS.Identity.Application.Common.Dtos.Roles;

public class RoleDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int UsersCount { get; set; }
    public int PermissionsCount { get; set; }
}
