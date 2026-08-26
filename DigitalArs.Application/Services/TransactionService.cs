using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;

namespace DigitalArs.Application.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TransactionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var transactions = await _unitOfWork.Repository<Transaction>().GetAllAsync(cancellationToken);
        return transactions.Select(ToDto).ToList();
    }

    public async Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.Repository<Transaction>().GetByIdAsync(id, cancellationToken);
        return transaction is null ? null : ToDto(transaction);
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto, CancellationToken cancellationToken = default)
    {
        var transaction = new Transaction
        {
            ID_Account = dto.AccountId,
            Type = dto.Type, // Deposit / Transfer_In / Transfer_Out
            Amount = dto.Amount,
            Date_Transaction = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Transaction>().AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(transaction);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.Repository<Transaction>().GetByIdAsync(id, cancellationToken);
        if (transaction is null) return false;

        _unitOfWork.Repository<Transaction>().Delete(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TransactionDto ToDto(Transaction transaction) =>
        new(transaction.ID_Transaction, transaction.ID_Account, transaction.Type, transaction.Amount, transaction.Date_Transaction);
}
