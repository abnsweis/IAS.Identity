using IAS.Identity.Domain.Common.Constants;

namespace IAS.Identity.Domain.Entities;

public class Permission : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}