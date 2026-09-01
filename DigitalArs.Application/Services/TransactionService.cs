using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;
using MapsterMapper;

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
    private readonly IMapper _mapper;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TransactionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var transactions = await _unitOfWork.Repository<Transaction>().GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<Transaction>, List<TransactionDto>>(transactions);
    }

    public async Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.Repository<Transaction>().GetByIdAsync(id, cancellationToken);
        return transaction is null ? null : _mapper.Map<Transaction, TransactionDto>(transaction);
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto, CancellationToken cancellationToken = default)
    {
        var transaction = _mapper.Map<CreateTransactionDto, Transaction>(dto);
        await _unitOfWork.Repository<Transaction>().AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<Transaction, TransactionDto>(transaction);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.Repository<Transaction>().GetByIdAsync(id, cancellationToken);
        if (transaction is null) return false;

        _unitOfWork.Repository<Transaction>().Delete(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
