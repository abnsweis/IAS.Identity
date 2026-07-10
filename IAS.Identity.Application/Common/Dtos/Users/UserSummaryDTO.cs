namespace IAS.Identity.Application.Common.Dtos.Users;

public class UserSummaryDTO 
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string? Phone { get; set; }
    public string Status { get; set; } = null!;
}