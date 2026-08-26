using DigitalArs.Domain.Enum; // Deposit, Transfer_In, Transfer_Out (Swagger lo muestra como enum)

namespace DigitalArs.Application.DTOs;

public record TransactionDto(int Id, int AccountId, TransactionType Type, decimal Amount, DateTime Date);

public record CreateTransactionDto(int AccountId, TransactionType Type, decimal Amount);
