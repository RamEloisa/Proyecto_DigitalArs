namespace DigitalArs.Application.DTOs;

/// Respuesta de cuenta (Price = saldo).
/// GET /api/accounts/{id}????
public record AccountDto(int Id, int UserId, string Name, decimal Price, DateTime Date);

/// Body de POST /api/accounts.
public record CreateAccountDto(int UserId, string Name, decimal Price);

/// Body de PUT /api/accounts/{id}.
public record UpdateAccountDto(string Name, decimal Price);

/// GET /api/accounts/me
public record AccountMeDto(int Id, decimal Price, DateTime Date);
