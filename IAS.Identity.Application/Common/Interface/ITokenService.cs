using IAS.Identity.Domain.Entities;

namespace IAS.Identity.Application.Common.Interface;

public interface ITokenService
{
    string GenerateToken(User user);

    string GenerateRefreshToken();
}