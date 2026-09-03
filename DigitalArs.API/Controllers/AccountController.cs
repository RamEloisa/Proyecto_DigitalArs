using DigitalArs.Application.Services;
using DigitalArs.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace DigitalArs.API.Controllers
{
    [ApiController]
    [Route("api/accounts/me")]
    [Tags("Accounts")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _account;

        public AccountController(IAccountService account)
        {
            _account = account;
        }

        [HttpGet]
        [EndpointSummary("Obtiene la cuenta del usuario autenticado")]
        [ProducesResponseType(typeof(AccountMeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountDto>> GetMeAccount(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromToken();

            var myAccount = await _account.GetMeAsync(userId, cancellationToken);

            return myAccount is null ? NotFound() : Ok(myAccount);

        }

        private int GetUserIdFromToken() 
        {
            var userIdClaim = User.FindFirst("userId");

            if(string.IsNullOrEmpty(userIdClaim?.Value) || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Token invalido: no es el id del usuario.");
            }

            return userId;
        }
   
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Obtiene una cuenta por id")]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var account = await _account.GetByIdAsync(id, cancellationToken);

            return account is null ? NotFound() : Ok(account);
        }
   
    }
}
