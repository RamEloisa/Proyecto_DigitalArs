using DigitalArs.Application.Services;
using DigitalArs.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DigitalArs.API.Controllers
{
    [ApiController]
    [Route("api/users/me")]
    [Tags("Users")]
    [Authorize]
    public class UsersMeController : ControllerBase
    {
        private readonly IUserMeService _userMeService;

        public UsersMeController(IUserMeService userMeService)
        {
            _userMeService = userMeService;
        }

        [HttpGet]
        [EndpointSummary("Obtiene los datos del usuario autenticado")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var userId = GetUserIdFromToken();
            var user = await _userMeService.GetMeAsync(userId);
            return Ok(user);
        }

        [HttpPut]
        [EndpointSummary("Actualiza los datos del usuario autenticado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateMeDto request)
        {
            var userId = GetUserIdFromToken();
            await _userMeService.UpdateMeAsync(userId, request);
            return NoContent();
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("userId");
            if (string.IsNullOrEmpty(userIdClaim?.Value) || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Token invalido: no es el id del usuario.");
            }
            return userId;
        }
    }
}