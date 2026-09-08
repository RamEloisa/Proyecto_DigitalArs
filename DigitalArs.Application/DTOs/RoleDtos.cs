namespace DigitalArs.Application.DTOs;

// Lo que Swagger muestra al listar/obtener un rol (sin la colección Users)
public record RoleDto(int Id, string Name);

// Body de POST /api/roles
public record CreateRoleDto(string Name);

// Body de PUT /api/roles/{id}
public record UpdateRoleDto(string Name);
