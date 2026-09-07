using DigitalArs.Application.Abstractions;
using DigitalArs.Application.DTOs;
using DigitalArs.Application.Exceptions;
using DigitalArs.Application.Realtime;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Enum;
using DigitalArs.Domain.Interfaces;
using MapsterMapper;
using System.Linq.Expressions;

namespace DigitalArs.Application.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResultDto<TransactionDto>> GetMinePagedAsync(
        int userId,
        TransactionQueryDto query,
        CancellationToken cancellationToken = default);
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto, CancellationToken cancellationToken = default);
    Task<TransferResultDto> TransferAsync(int sourceUserId, TransferDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public TransactionService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IRealtimeNotifier realtimeNotifier)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _realtimeNotifier = realtimeNotifier;
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

    public async Task<PagedResultDto<TransactionDto>> GetMinePagedAsync(
        int userId,
        TransactionQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var predicate = BuildMineFilter(userId, query);
        Expression<Func<Transaction, TransactionDto>> selector = t =>
            new TransactionDto(t.ID_Transaction, t.ID_Account, t.Type, t.Amount, t.Date_Transaction);

        var (items, totalItems) = await _unitOfWork.Repository<Transaction>()
            .GetPagedProjectedAsync(
                query.Page,
                query.PageSize,
                selector,
                predicate,
                t => t.Date_Transaction,
                descending: true,
                cancellationToken);

        return PagedResultDto<TransactionDto>.Create(items, query.Page, query.PageSize, totalItems);
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
            if (destination is null)
            {
                throw new DestinationAccountNotFoundException();
            }

            var owner = destination.User
                ?? (await _unitOfWork.Repository<User>().GetByIdAsync(destination.ID_User, cancellationToken));
            if (owner is null || !owner.IsActive)
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

            var outgoingNotification = AccountNotificationFactory.Create(
                source.ID_User,
                TransactionType.Transfer_Out,
                dto.Amount);
            var incomingNotification = AccountNotificationFactory.Create(
                destination.ID_User,
                TransactionType.Transfer_In,
                dto.Amount);

            var notifications = _unitOfWork.Repository<Notification>();
            await notifications.AddAsync(outgoingNotification, cancellationToken);
            await notifications.AddAsync(incomingNotification, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            await _realtimeNotifier.NotifyUserAsync(
                source.ID_User,
                AccountNotificationFactory.ToEvent(outgoingNotification, source.Price),
                cancellationToken);
            await _realtimeNotifier.NotifyUserAsync(
                destination.ID_User,
                AccountNotificationFactory.ToEvent(incomingNotification, destination.Price),
                cancellationToken);

            return new TransferResultDto(
                _mapper.Map<Transaction, TransactionDto>(transferOut),
                _mapper.Map<Transaction, TransactionDto>(transferIn));
        }
        catch
        {
            await RollbackSafelyAsync(cancellationToken);
            throw;
        }
    }

    private async Task RollbackSafelyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
        }
        catch (Exception)
        {
            // El rollback no debe tapar la excepción de negocio (p. ej. saldo insuficiente → 400).
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

    private static Expression<Func<Transaction, bool>> BuildMineFilter(int userId, TransactionQueryDto query)
    {
        var type = query.Type;
        var fromDate = query.FromDate;
        var toDate = NormalizeToDateInclusive(query.ToDate);
        var minAmount = query.MinAmount;
        var maxAmount = query.MaxAmount;

        var hasType = type.HasValue;
        var hasFrom = fromDate.HasValue;
        var hasTo = toDate.HasValue;
        var hasMin = minAmount.HasValue;
        var hasMax = maxAmount.HasValue;

        return t =>
            t.Account.ID_User == userId &&
            (!hasType || t.Type == type!.Value) &&
            (!hasFrom || t.Date_Transaction >= fromDate!.Value) &&
            (!hasTo || t.Date_Transaction <= toDate!.Value) &&
            (!hasMin || t.Amount >= minAmount!.Value) &&
            (!hasMax || t.Amount <= maxAmount!.Value);
    }

    // Si ToDate llega a medianoche (solo fecha), incluye todo ese día.
    private static DateTime? NormalizeToDateInclusive(DateTime? toDate)
    {
        if (toDate is null)
        {
            return null;
        }

        var value = toDate.Value;
        return value.TimeOfDay == TimeSpan.Zero
            ? value.Date.AddDays(1).AddTicks(-1)
            : value;
    }
}
