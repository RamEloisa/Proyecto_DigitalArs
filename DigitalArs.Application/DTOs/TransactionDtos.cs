using DigitalArs.Domain.Enum;

namespace DigitalArs.Application.DTOs;

/// Respuesta de transacción.
public record TransactionDto(int Id, int AccountId, TransactionType Type, decimal Amount, DateTime Date);

/// Body de POST /api/transactions.
public record CreateTransactionDto(int AccountId, TransactionType Type, decimal Amount);

/// Body de POST /api/transactions/transfer.
public record TransferDto(int DestinationAccountId, decimal Amount);

/// Respuesta de transferencia: el par Transfer_Out + Transfer_In.
public record TransferResultDto(TransactionDto TransferOut, TransactionDto TransferIn);