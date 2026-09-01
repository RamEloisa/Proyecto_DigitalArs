using Microsoft.AspNetCore.Mvc;
using DigitalArs.Application.DTOs.Auth;
using DigitalArs.Application.Security;
using Microsoft.AspNetCore.Authorization;

namespace DigitalArs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                message = "Credenciales inválidas."
            });
        }
    }
}
