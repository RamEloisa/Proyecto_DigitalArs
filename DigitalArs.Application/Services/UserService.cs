using DigitalArs.Application.DTOs;
using DigitalArs.Application.Exceptions;
using DigitalArs.Application.Security;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;
using MapsterMapper;
using System.Linq.Expressions;

namespace DigitalArs.Application.Services;

public interface IUserService
{
    Task<PagedResultDto<UserDto>> GetPagedAsync(UserQueryDto query, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedResultDto<UserDto>> GetPagedAsync(
        UserQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var predicate = BuildFilter(query);
        var (users, totalCount) = await _unitOfWork.Repository<User>()
            .GetPagedAsync(query.Page, query.PageSize, predicate, cancellationToken);

        var items = _mapper.Map<IReadOnlyList<User>, List<UserDto>>(users);
        return new PagedResultDto<UserDto>(items, totalCount, query.Page, query.PageSize);
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(id, cancellationToken);
        return user is null ? null : _mapper.Map<User, UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureEmailIsUniqueAsync(dto.Email, excludeUserId: null, cancellationToken);
        await EnsureRoleExistsAsync(dto.RoleId, cancellationToken);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = _mapper.Map<CreateUserDto, User>(dto);
            user.IsActive = true;
            user.Password_Hasheada = _passwordHasher.Hash(dto.Password);
            user.Account = new Account
            {
                Name = "Cuenta Principal",
                Price = 0m,
                Date = DateTime.UtcNow
            };

            await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return _mapper.Map<User, UserDto>(user);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(id, cancellationToken);
        if (user is null) return false;

        await EnsureEmailIsUniqueAsync(dto.Email, excludeUserId: id, cancellationToken);
        await EnsureRoleExistsAsync(dto.RoleId, cancellationToken);

        _mapper.Map<UpdateUserDto, User>(dto, user);
        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(id, cancellationToken);
        if (user is null) return false;

        if (!user.IsActive)
        {
            return true;
        }

        user.IsActive = false;
        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureEmailIsUniqueAsync(
        string email,
        int? excludeUserId,
        CancellationToken cancellationToken)
    {
        var matches = await _unitOfWork.Repository<User>().FindAsync(
            u => u.Email == email,
            cancellationToken);

        if (matches.Any(u => !excludeUserId.HasValue || u.ID_User != excludeUserId.Value))
        {
            throw new DuplicateEmailException();
        }
    }

    private async Task EnsureRoleExistsAsync(int roleId, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(roleId, cancellationToken);
        if (role is null)
        {
            throw new InvalidRoleException();
        }
    }

    private static Expression<Func<User, bool>>? BuildFilter(UserQueryDto query)
    {
        var name = query.Name?.Trim();
        var email = query.Email?.Trim();
        var roleId = query.RoleId;
        var isActive = query.IsActive;

        var hasName = !string.IsNullOrWhiteSpace(name);
        var hasEmail = !string.IsNullOrWhiteSpace(email);
        var hasRoleId = roleId.HasValue;
        var hasIsActive = isActive.HasValue;

        if (!hasName && !hasEmail && !hasRoleId && !hasIsActive)
        {
            return null;
        }

        return u =>
            (!hasName || u.Full_Name.Contains(name!)) &&
            (!hasEmail || u.Email.Contains(email!)) &&
            (!hasRoleId || u.ID_Role == roleId!.Value) &&
            (!hasIsActive || u.IsActive == isActive!.Value);
    }
}
