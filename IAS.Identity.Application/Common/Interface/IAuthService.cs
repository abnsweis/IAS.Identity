using IAS.Identity.Application.Common.Dtos.Auth;
using IAS.Identity.Application.Common.Dtos.Users;

namespace IAS.Identity.Application.Common.Interface;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequestDto loginRequest);

    Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto refreshTokenRequest);

    Task LogoutAsync(string refreshToken);

    Task RevokeTokens(string userId);

    Task<RegisterResponse> Register(RegisterUserDto registerUserDto);

    Task<UserDto> GetMe();
}