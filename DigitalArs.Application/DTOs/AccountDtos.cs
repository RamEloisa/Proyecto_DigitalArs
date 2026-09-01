namespace DigitalArs.Application.DTOs;

/// Respuesta de cuenta (Price = saldo).
public record AccountDto(int Id, int UserId, string Name, decimal Price, DateTime Date);

/// Body de POST /api/accounts.
public record CreateAccountDto(int UserId, string Name, decimal Price);

/// Body de PUT /api/accounts/{id}.
public record UpdateAccountDto(string Name, decimal Price);