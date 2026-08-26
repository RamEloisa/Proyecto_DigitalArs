namespace DigitalArs.Application.DTOs;

// Respuesta pública: nunca incluye Password_Hasheada
public record UserDto(int Id, string FullName, string Email, string Dni, string Alias, int RoleId);

// Password se guarda en Password_Hasheada (el hash real viene después)
public record CreateUserDto(string FullName, string Email, string Password, string Dni, string Alias, int RoleId);

public record UpdateUserDto(string FullName, string Email, string Dni, string Alias, int RoleId);
