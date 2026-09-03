using DigitalArs.Application.Services;
using DigitalArs.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DigitalArs.API.Controllers
{
    [ApiController]
    [Route("api/users/me")]
    [Tags("Users")]
    public class UsersMeController : ControllerBase
    {
        private readonly IUserMeService _userMeService;

        public UsersMeController(IUserMeService userMeService)
        {
            _userMeService = userMeService;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var userId = GetUserIdFromToken();
            var user = await _userMeService.GetMeAsync(userId);
            return Ok(user);
        }

        [HttpPut]
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