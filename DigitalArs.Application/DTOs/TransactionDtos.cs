using DigitalArs.Domain.Enum;

namespace DigitalArs.Application.DTOs;

/// Respuesta de transacción (historial de movimientos: depósitos y transferencias).
public record TransactionDto(int Id, int AccountId, TransactionType Type, decimal Amount, DateTime Date)
{
    public string Description => Type switch
    {
        TransactionType.Deposit => "Depósito",
        TransactionType.Transfer_In => "Transferencia recibida",
        TransactionType.Transfer_Out => "Transferencia enviada",
        _ => Type.ToString()
    };
}

/// Query de GET /api/transactions/me (paginación + filtros).
public sealed class TransactionQueryDto
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public TransactionType? Type { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public decimal? MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
}

/// Body de POST /api/transactions.
public record CreateTransactionDto(int AccountId, TransactionType Type, decimal Amount);

/// Body de POST /api/transactions/transfer.
public record TransferDto(int DestinationAccountId, decimal Amount);

/// Respuesta de transferencia: el par Transfer_Out + Transfer_In.
public record TransferResultDto(TransactionDto TransferOut, TransactionDto TransferIn);