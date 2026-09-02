using DigitalArs.Application.DTOs;
using DigitalArs.Application.Exceptions;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Enum;
using DigitalArs.Domain.Interfaces;
using MapsterMapper;

namespace DigitalArs.Application.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto, CancellationToken cancellationToken = default);
    Task<TransferResultDto> TransferAsync(int sourceUserId, TransferDto dto, CancellationToken cancellationToken = default);
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

    public async Task<TransferResultDto> TransferAsync(
        int sourceUserId,
        TransferDto dto,
        CancellationToken cancellationToken = default)
    {
        var accounts = _unitOfWork.Repository<Account>();
        var transactions = _unitOfWork.Repository<Transaction>();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var sourceMatches = await accounts.FindAsync(
                a => a.ID_User == sourceUserId,
                cancellationToken);
            var source = sourceMatches.FirstOrDefault()
                ?? throw new SourceAccountNotFoundException();

            var destinationMatches = await accounts.FindAsync(
                a => a.ID_Account == dto.DestinationAccountId,
                cancellationToken,
                a => a.User);
            var destination = destinationMatches.FirstOrDefault();

            if (destination is null || !destination.User.IsActive)
            {
                throw new DestinationAccountNotFoundException();
            }

            if (destination.ID_Account == source.ID_Account)
            {
                throw new SelfTransferException();
            }

            if (source.Price < dto.Amount)
            {
                throw new InsufficientBalanceException();
            }

            source.Price -= dto.Amount;
            destination.Price += dto.Amount;
            accounts.Update(source);
            accounts.Update(destination);

            var occurredAt = DateTime.UtcNow;
            var transferOut = new Transaction
            {
                ID_Account = source.ID_Account,
                Type = TransactionType.Transfer_Out,
                Amount = dto.Amount,
                Date_Transaction = occurredAt
            };
            var transferIn = new Transaction
            {
                ID_Account = destination.ID_Account,
                Type = TransactionType.Transfer_In,
                Amount = dto.Amount,
                Date_Transaction = occurredAt
            };

            await transactions.AddAsync(transferOut, cancellationToken);
            await transactions.AddAsync(transferIn, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return new TransferResultDto(
                _mapper.Map<Transaction, TransactionDto>(transferOut),
                _mapper.Map<Transaction, TransactionDto>(transferIn));
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
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
