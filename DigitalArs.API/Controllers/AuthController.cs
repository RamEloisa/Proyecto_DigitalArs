using Microsoft.AspNetCore.Mvc;
using DigitalArs.Application.Security;
using Microsoft.AspNetCore.Authorization;
namespace DigitalArs.API.Controllers;
using DigitalArs.Application.DTOs;

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
    [EndpointSummary("Inicia sesión con credenciales de usuario (email y contraseña) y devuelve un token JWT.")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
