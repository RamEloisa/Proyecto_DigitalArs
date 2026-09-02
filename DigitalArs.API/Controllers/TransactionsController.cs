using DigitalArs.Application.DTOs;
using DigitalArs.Application.Exceptions;
using DigitalArs.Application.Security;
using DigitalArs.Application.Services;
using DigitalArs.API.Filters;
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
        try
        {
            var sourceUserId = CurrentUserHelper.GetUserId(User);
            var result = await _transactions.TransferAsync(sourceUserId, dto, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "El token no contiene un userId válido." });
        }
        catch (SourceAccountNotFoundException ex)
        {
            return ValidationError("Account", ex.Message);
        }
        catch (SelfTransferException ex)
        {
            return ValidationError("DestinationAccountId", ex.Message);
        }
        catch (InsufficientBalanceException ex)
        {
            return ValidationError("Amount", ex.Message);
        }
        catch (DestinationAccountNotFoundException)
        {
            return NotFound();
        }
    }

    private BadRequestObjectResult ValidationError(string field, string message) =>
        ValidationErrorResponseFactory.From(
            [new ValidationErrorDto(field, message)]);
}
