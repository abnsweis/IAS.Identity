namespace IAS.Identity.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset ExpiresOnUTC { get; set; }
    public User? User { get; set; }
}