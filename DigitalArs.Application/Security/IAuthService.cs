using DigitalArs.Application.DTOs.Auth;

namespace DigitalArs.Application.Security;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}