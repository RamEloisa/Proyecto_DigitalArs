namespace DigitalArs.Application.DTOs;

/// Respuesta de usuario: nunca incluye Password_Hasheada.
public record UserDto(int Id, string FullName, string Email, string Dni, string Alias, int RoleId);

/// Body de POST /api/users. Password se persiste como hash, no se devuelve.
public record CreateUserDto(string FullName, string Email, string Password, string Dni, string Alias, int RoleId);

/// Body de PUT /api/users/{id}. No permite cambiar la contraseña.
public record UpdateUserDto(string FullName, string Email, string Dni, string Alias, int RoleId);
