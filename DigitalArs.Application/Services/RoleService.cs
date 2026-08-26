using DigitalArs.Application.DTOs; // RoleDto y bodies de create/update
using DigitalArs.Domain.Entities; // Entidad Role (nunca se expone tal cual en la API)
using DigitalArs.Domain.Interfaces; // IUnitOfWork: los servicios no ven el DbContext

namespace DigitalArs.Application.Services;

public interface IRoleService
{
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default); // GET lista
    Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default); // GET por id; null = 404
    Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default); // POST
    Task<bool> UpdateAsync(int id, UpdateRoleDto dto, CancellationToken cancellationToken = default); // PUT; false = 404
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default); // DELETE; false = 404
}

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork; // Única dependencia de persistencia

    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork; // Lo inyecta el DI (Scoped, mismo request que el DbContext)
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _unitOfWork.Repository<Role>().GetAllAsync(cancellationToken); // SELECT * FROM Roles
        return roles.Select(ToDto).ToList(); // Mapea entidad → DTO para Swagger/JSON
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id, cancellationToken); // Busca por PK
        return role is null ? null : ToDto(role); // null → el controller responde 404
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = new Role { Name = dto.Name }; // PK la genera SQL Server al guardar
        await _unitOfWork.Repository<Role>().AddAsync(role, cancellationToken); // Estado Added
        await _unitOfWork.SaveChangesAsync(cancellationToken); // INSERT real
        return ToDto(role); // Ya tiene ID_Role asignado por EF
    }

    public async Task<bool> UpdateAsync(int id, UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id, cancellationToken);
        if (role is null) return false; // No existe

        role.Name = dto.Name; // Cambia el campo tracked
        _unitOfWork.Repository<Role>().Update(role); // Marca Modified
        await _unitOfWork.SaveChangesAsync(cancellationToken); // UPDATE
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id, cancellationToken);
        if (role is null) return false;

        _unitOfWork.Repository<Role>().Delete(role); // Estado Deleted
        await _unitOfWork.SaveChangesAsync(cancellationToken); // DELETE
        return true;
    }

    private static RoleDto ToDto(Role role) => new(role.ID_Role, role.Name); // API usa Id, no ID_Role
}
