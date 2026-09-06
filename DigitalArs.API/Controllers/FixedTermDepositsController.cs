using DigitalArs.Application.DTOs;
using DigitalArs.Application.Security;
using DigitalArs.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalArs.API.Controllers;

[ApiController]
[Route("api/fixed-deposits")]
[Tags("FixedTermDeposits")]
[Authorize]
public class FixedTermDepositsController : ControllerBase
{
    private readonly IFixedTermDepositService _deposits;

    public FixedTermDepositsController(IFixedTermDepositService deposits)
    {
        _deposits = deposits;
    }

    [HttpPost]
    [EndpointSummary("Constituye un plazo fijo debitando el saldo de la cuenta")]
    [ProducesResponseType(typeof(FixedTermDepositDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FixedTermDepositDto>> Create(
        [FromBody] CreateFixedTermDepositDto dto,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserHelper.GetUserId(User);
        var result = await _deposits.CreateAsync(userId, dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("me")]
    [EndpointSummary("Lista los plazos fijos del usuario autenticado (liquida los vencidos antes de devolverlos)")]
    [ProducesResponseType(typeof(IReadOnlyList<FixedTermDepositMeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<FixedTermDepositMeDto>>> GetMine(
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserHelper.GetUserId(User);
        var result = await _deposits.GetMineAsync(userId, cancellationToken);
        return Ok(result);
    }
}
