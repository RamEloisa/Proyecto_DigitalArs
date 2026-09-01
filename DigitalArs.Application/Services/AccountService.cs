using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;
using MapsterMapper;

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
    private readonly IMapper _mapper;

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
}
