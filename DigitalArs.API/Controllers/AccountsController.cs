using DigitalArs.Application.DTOs;
using DigitalArs.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; //para autorizar la ruta

namespace DigitalArs.API.Controllers;

[ApiController]
[Route("api/accounts")]
[Tags("Accounts")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accounts;

    public AccountsController(IAccountService accounts)
    {
        _accounts = accounts;
    }

    [HttpGet]
    [EndpointSummary("Lista todas las cuentas")]
    [ProducesResponseType(typeof(IReadOnlyList<AccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AccountDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _accounts.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Obtiene una cuenta por id")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var account = await _accounts.GetByIdAsync(id, cancellationToken);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpPost]
    [EndpointSummary("Crea una cuenta (1:1 con un usuario)")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountDto dto, CancellationToken cancellationToken)
    {
        var created = await _accounts.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Actualiza nombre y saldo de una cuenta")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountDto dto, CancellationToken cancellationToken)
    {
        var updated = await _accounts.UpdateAsync(id, dto, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Elimina una cuenta")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _accounts.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
