using DigitalArs.Application.Abstractions;
using DigitalArs.Application.DTOs;
using DigitalArs.Application.Exceptions;
using DigitalArs.Application.Options;
using DigitalArs.Application.Realtime;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Enum;
using DigitalArs.Domain.Interfaces;
using MapsterMapper;
using Microsoft.Extensions.Options;

namespace DigitalArs.Application.Services;

public interface IFixedTermDepositService
{
    Task<FixedTermDepositDto> CreateAsync(
        int userId,
        CreateFixedTermDepositDto dto,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FixedTermDepositMeDto>> GetMineAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task SettleMaturedAsync(
        int? userId = null,
        CancellationToken cancellationToken = default);
}

public class FixedTermDepositService : IFixedTermDepositService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly FixedTermDepositSettings _settings;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public FixedTermDepositService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOptions<FixedTermDepositSettings> options,
        IRealtimeNotifier realtimeNotifier)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _settings = options.Value;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task<FixedTermDepositDto> CreateAsync(
        int userId,
        CreateFixedTermDepositDto dto,
        CancellationToken cancellationToken = default)
    {
        var accounts = _unitOfWork.Repository<Account>();
        var deposits = _unitOfWork.Repository<FixedTermDeposit>();
        var transactions = _unitOfWork.Repository<Transaction>();

        var accountMatches = await accounts.FindAsync(
            a => a.ID_User == userId,
            cancellationToken);
        var account = accountMatches.FirstOrDefault()
            ?? throw new KeyNotFoundException(
                $"Cuenta del usuario con ID {userId} no encontrada.");

        if (account.Price < dto.Amount)
        {
            throw new InsufficientBalanceException();
        }

        var createdAt = DateTime.UtcNow;
        var interestAmount = CalculateInterest(dto.Amount, _settings.AnnualRate, dto.TermDays);
        var deposit = new FixedTermDeposit
        {
            ID_Account = account.ID_Account,
            Amount = dto.Amount,
            AnnualRate = _settings.AnnualRate,
            TermDays = dto.TermDays,
            InterestAmount = interestAmount,
            FinalAmount = dto.Amount + interestAmount,
            CreatedAt = createdAt,
            MaturityDate = createdAt.AddDays(dto.TermDays),
            Status = FixedTermDepositStatus.Active
        };

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            account.Price -= dto.Amount;
            accounts.Update(account);

            await deposits.AddAsync(deposit, cancellationToken);
            await transactions.AddAsync(new Transaction
            {
                ID_Account = account.ID_Account,
                Type = TransactionType.FixedTerm_Out,
                Amount = dto.Amount,
                Date_Transaction = createdAt
            }, cancellationToken);

            var notification = AccountNotificationFactory.Create(
                userId,
                TransactionType.FixedTerm_Out,
                dto.Amount);

            await _unitOfWork
                .Repository<Notification>()
                .AddAsync(notification, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            await _realtimeNotifier.NotifyUserAsync(
                userId,
                AccountNotificationFactory.ToEvent(notification, account.Price),
                cancellationToken);

            return _mapper.Map<FixedTermDeposit, FixedTermDepositDto>(deposit);
        }
        catch
        {
            await RollbackSafelyAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IReadOnlyList<FixedTermDepositMeDto>> GetMineAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        await SettleMaturedAsync(userId, cancellationToken);

        var deposits = await _unitOfWork.Repository<FixedTermDeposit>()
            .FindAsync(d => d.Account.ID_User == userId, cancellationToken);

        return _mapper.Map<IReadOnlyList<FixedTermDeposit>, List<FixedTermDepositMeDto>>(
            deposits.OrderByDescending(d => d.CreatedAt).ToList());
    }

    public async Task SettleMaturedAsync(
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var deposits = _unitOfWork.Repository<FixedTermDeposit>();
        var accounts = _unitOfWork.Repository<Account>();
        var transactions = _unitOfWork.Repository<Transaction>();

        var matured = await deposits.FindAsync(
            d => d.Status == FixedTermDepositStatus.Active
                && d.MaturityDate <= now
                && (!userId.HasValue || d.Account.ID_User == userId.Value),
            cancellationToken,
            d => d.Account);

        if (matured.Count == 0)
        {
            return;
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var pendingEvents = new List<(Notification Notification, decimal Balance)>();
            var notifications = _unitOfWork.Repository<Notification>();

            foreach (var deposit in matured)
            {
                if (deposit.Status != FixedTermDepositStatus.Active)
                {
                    continue;
                }

                var account = deposit.Account
                    ?? throw new InvalidOperationException(
                        $"La cuenta del plazo fijo {deposit.ID_FixedTermDeposit} no está disponible.");

                account.Price += deposit.FinalAmount;
                accounts.Update(account);

                deposit.Status = FixedTermDepositStatus.Settled;
                deposit.SettledAt = now;
                deposits.Update(deposit);

                await transactions.AddAsync(new Transaction
                {
                    ID_Account = account.ID_Account,
                    Type = TransactionType.FixedTerm_In,
                    Amount = deposit.FinalAmount,
                    Date_Transaction = now
                }, cancellationToken);

                var notification = AccountNotificationFactory.Create(
                    account.ID_User,
                    TransactionType.FixedTerm_In,
                    deposit.FinalAmount);

                await notifications.AddAsync(notification, cancellationToken);
                pendingEvents.Add((notification, account.Price));
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            foreach (var (notification, balance) in pendingEvents)
            {
                await _realtimeNotifier.NotifyUserAsync(
                    notification.ID_User,
                    AccountNotificationFactory.ToEvent(notification, balance),
                    cancellationToken);
            }
        }
        catch
        {
            await RollbackSafelyAsync(cancellationToken);
            throw;
        }
    }

    internal static decimal CalculateInterest(decimal amount, decimal annualRate, int termDays)
    {
        return Math.Round(amount * annualRate * termDays / 365m, 2, MidpointRounding.AwayFromZero);
    }

    private async Task RollbackSafelyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
        }
        catch (Exception)
        {
            // El rollback no debe tapar la excepción de negocio.
        }
    }
}
