namespace IAS.Identity.Application.Common.Dtos.Permissions;

public class PermissionSummaryDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int RolesCount { get; set; }
    public int UsersCount { get; set; }
}