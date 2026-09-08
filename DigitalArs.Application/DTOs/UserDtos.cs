namespace DigitalArs.Application.DTOs;

/// Respuesta de usuario: nunca incluye Password_Hasheada.
public record UserDto(int Id, string FullName, string Email, string Dni, string Alias, int RoleId, bool IsActive, int? AccountId);

/// Resultado de búsqueda por alias: no expone email, DNI ni rol.
public record UserLookupDto(int Id, string FullName, string Alias, int? AccountId);

/// Body de POST /api/users. Password se persiste como hash, no se devuelve.
public record CreateUserDto(string FullName, string Email, string Password, string Dni, string Alias, int RoleId);

/// Body de PUT /api/users/{id}. No permite cambiar la contraseña.
public record UpdateUserDto(string FullName, string Email, string Dni, string Alias, int RoleId, bool IsActive);

public sealed class UserQueryDto
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Name { get; init; }
    public string? Email { get; init; }
    public int? RoleId { get; init; }
    public bool? IsActive { get; init; }
}

/// Envelope de listados paginados.
public record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages)
{
    public static PagedResultDto<T> Create(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalItems)
    {
        var totalPages = pageSize <= 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResultDto<T>(items, page, pageSize, totalItems, totalPages);
    }
}
public record UpdateMeDto(string FullName, string Email, string Dni, string Alias, string? CurrentPassword, string? NewPassword);
