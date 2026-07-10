namespace IAS.Identity.Application.Common.Dtos.Permissions;

public class BasePermissionRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}