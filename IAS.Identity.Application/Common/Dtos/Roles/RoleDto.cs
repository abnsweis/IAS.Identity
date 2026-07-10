namespace IAS.Identity.Application.Common.Dtos.Roles;

public class RoleDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int UsersCount { get; set; }
    public int PermissionsCount { get; set; }
}
