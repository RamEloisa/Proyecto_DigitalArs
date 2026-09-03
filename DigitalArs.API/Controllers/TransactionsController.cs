using DigitalArs.Application.DTOs;
using DigitalArs.Application.Security;
using DigitalArs.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalArs.API.Controllers;

[ApiController]
[Route("api/transactions")]
[Tags("Transactions")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactions;

    public TransactionsController(ITransactionService transactions)
    {
        _transactions = transactions;
    }

    [HttpGet("me")]
    [EndpointSummary("Historial de movimientos del usuario autenticado (paginado, ordenado por fecha desc)")]
    [ProducesResponseType(typeof(PagedResultDto<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResultDto<TransactionDto>>> GetMine(
        [FromQuery] TransactionQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = CurrentUserHelper.GetUserId(User);
            var result = await _transactions.GetMinePagedAsync(userId, query, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "El token no contiene un userId válido." });
        }
    }

    [HttpPost("transfer")]
    [EndpointSummary("Transfiere fondos a otra cuenta (débito origen + crédito destino, atómico)")]
    [ProducesResponseType(typeof(TransferResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransferResultDto>> Transfer(
        [FromBody] TransferDto dto,
        CancellationToken cancellationToken)
    {
        var sourceUserId = CurrentUserHelper.GetUserId(User);
        var result = await _transactions.TransferAsync(sourceUserId, dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
