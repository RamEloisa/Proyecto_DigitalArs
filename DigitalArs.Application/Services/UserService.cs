using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;

namespace DigitalArs.Application.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork; // Nunca DigitalArsDbContext

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Repository<User>().GetAllAsync(cancellationToken);
        return users.Select(ToDto).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(id, cancellationToken);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Full_Name = dto.FullName,
            Email = dto.Email,
            Password_Hasheada = dto.Password, // TODO: reemplazar por hash (BCrypt/Identity)
            DNI = dto.Dni,
            Alias = dto.Alias,
            ID_Role = dto.RoleId
        };

        await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(user);
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(id, cancellationToken);
        if (user is null) return false;

        user.Full_Name = dto.FullName;
        user.Email = dto.Email;
        user.DNI = dto.Dni;
        user.Alias = dto.Alias;
        user.ID_Role = dto.RoleId;

        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(id, cancellationToken);
        if (user is null) return false;

        _unitOfWork.Repository<User>().Delete(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static UserDto ToDto(User user) =>
        new(user.ID_User, user.Full_Name, user.Email, user.DNI, user.Alias, user.ID_Role);
}
