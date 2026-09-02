namespace DigitalArs.Application.DTOs;

/// Respuesta de usuario: nunca incluye Password_Hasheada.
public record UserDto(int Id, string FullName, string Email, string Dni, string Alias, int RoleId, bool IsActive);

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
public record PagedResultDto<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
public record UpdateMeDto(string FullName, string Email, string Dni, string Alias, string? CurrentPassword, string? NewPassword);
