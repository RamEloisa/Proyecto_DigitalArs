using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;
using MapsterMapper;
using DigitalArs.Domain.Enum;

namespace DigitalArs.Application.Services;

public interface IAccountService
{
    Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AccountDto> CreateAsync(CreateAccountDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateAccountDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    // HU 14
    Task<AccountMeDto?> GetMeAsync(int userId, CancellationToken cancellationToken=default);
    // HU 15
    Task DepositAsync(int userId, DepositDto dto, CancellationToken cancellationToken = default);

}

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    //deposito maximo permitido
    private const decimal MaxDepositAmount = 1000000m;

    public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _unitOfWork.Repository<Account>().GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<Account>, List<AccountDto>>(accounts);
    }

    public async Task<AccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(id, cancellationToken);
        return account is null ? null : _mapper.Map<Account, AccountDto>(account);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto dto, CancellationToken cancellationToken = default)
    {
        var account = _mapper.Map<CreateAccountDto, Account>(dto);
        await _unitOfWork.Repository<Account>().AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<Account, AccountDto>(account);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAccountDto dto, CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(id, cancellationToken);
        if (account is null) return false;

        _mapper.Map<UpdateAccountDto, Account>(dto, account);
        _unitOfWork.Repository<Account>().Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(id, cancellationToken);
        if (account is null) return false;

        _unitOfWork.Repository<Account>().Delete(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<AccountMeDto?> GetMeAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<Account>()
            .FirstOrDefaultAsync(
                a => a.ID_User == userId,
                a => new AccountMeDto(
                    a.ID_Account,
                    a.Price,
                    a.Date),
                cancellationToken);
    }

    public async Task DepositAsync(int userId, DepositDto dto, CancellationToken cancellationToken = default)
    {
        var accounts = await _unitOfWork
        .Repository<Account>()
        .FindAsync(
            a => a.ID_User == userId,
            cancellationToken);

    var account = accounts.FirstOrDefault();

    if (account is null)
    {
        throw new KeyNotFoundException(
            $"Cuenta del usuario con ID {userId} no encontrada.");
    }

    if (dto.Amount > MaxDepositAmount)
    {
        throw new InvalidOperationException(
            $"El monto máximo por depósito es de {MaxDepositAmount}.");
    }

    await _unitOfWork.BeginTransactionAsync(cancellationToken);

    try
    {
        account.Price += dto.Amount;

        var transaction = new Transaction
        {
            ID_Account = account.ID_Account,
            Type = TransactionType.Deposit,
            Amount = dto.Amount
        };

        await _unitOfWork
            .Repository<Transaction>()
            .AddAsync(transaction, cancellationToken);

        _unitOfWork
            .Repository<Account>()
            .Update(account);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
    catch
    {
        await _unitOfWork.RollbackAsync(cancellationToken);
        throw;
    }
    }
}

