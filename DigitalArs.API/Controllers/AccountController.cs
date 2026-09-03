using DigitalArs.Application.Services;
using DigitalArs.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace DigitalArs.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Tags("Accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accounts;

        public AccountController(IAccountService accounts)
        {
            _accounts = accounts;
        }

        [HttpGet("me")]
        [Authorize]
        [EndpointSummary("Obtiene la cuenta del usuario autenticado")]
        [ProducesResponseType(typeof(AccountMeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountDto>> GetMeAccount(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromToken();

            var myAccount = await _accounts.GetMeAsync(userId, cancellationToken);

            return myAccount is null ? NotFound() : Ok(myAccount);

        }

        [HttpPost("deposit")]
        [Authorize]
        [EndpointSummary("Deposita en la cuenta del usuario autenticado y lo registra en el historial de movimientos")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDto>> Deposit([FromBody] DepositDto dto, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromToken();

            try
            {
                var result = await _accounts.DepositAsync(
                    userId,
                    dto,
                    cancellationToken);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
            var account = await _accounts.GetByIdAsync(id, cancellationToken);

            return account is null ? NotFound() : Ok(account);
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
