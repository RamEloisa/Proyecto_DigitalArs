using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;
using MapsterMapper;

namespace DigitalArs.Application.Services;

public interface IRoleService
{
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateRoleDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _unitOfWork.Repository<Role>().GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<Role>, List<RoleDto>>(roles);
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id, cancellationToken);
        return role is null ? null : _mapper.Map<Role, RoleDto>(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = _mapper.Map<CreateRoleDto, Role>(dto);
        await _unitOfWork.Repository<Role>().AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<Role, RoleDto>(role);
    }

    public async Task<bool> UpdateAsync(int id, UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id, cancellationToken);
        if (role is null) return false;

        _mapper.Map<UpdateRoleDto, Role>(dto, role);
        _unitOfWork.Repository<Role>().Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id, cancellationToken);
        if (role is null) return false;

        _unitOfWork.Repository<Role>().Delete(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
