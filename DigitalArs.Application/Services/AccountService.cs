using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;

namespace DigitalArs.Application.Services;

public interface IAccountService
{
    Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AccountDto> CreateAsync(CreateAccountDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateAccountDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _unitOfWork.Repository<Account>().GetAllAsync(cancellationToken);
        return accounts.Select(ToDto).ToList();
    }

    public async Task<AccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(id, cancellationToken);
        return account is null ? null : ToDto(account);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto dto, CancellationToken cancellationToken = default)
    {
        var account = new Account
        {
            ID_User = dto.UserId, // Relación 1:1 con User
            Name = dto.Name,
            Price = dto.Price, // Saldo inicial
            Date = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Account>().AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(account);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAccountDto dto, CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(id, cancellationToken);
        if (account is null) return false;

        account.Name = dto.Name;
        account.Price = dto.Price;

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

    private static AccountDto ToDto(Account account) =>
        new(account.ID_Account, account.ID_User, account.Name, account.Price, account.Date);
}
