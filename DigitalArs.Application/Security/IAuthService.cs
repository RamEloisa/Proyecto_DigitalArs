using DigitalArs.Application.DTOs;

namespace DigitalArs.Application.Security;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}