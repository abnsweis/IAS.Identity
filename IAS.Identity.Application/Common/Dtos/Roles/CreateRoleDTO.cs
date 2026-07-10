namespace IAS.Identity.Application.Common.Dtos.Roles;

public class CreateRoleDTO
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}