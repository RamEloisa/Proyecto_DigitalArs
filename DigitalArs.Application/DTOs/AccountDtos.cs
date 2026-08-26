namespace DigitalArs.Application.DTOs;

// Price = saldo de la cuenta (nombre de la entidad de dominio)
public record AccountDto(int Id, int UserId, string Name, decimal Price, DateTime Date);

public record CreateAccountDto(int UserId, string Name, decimal Price);

public record UpdateAccountDto(string Name, decimal Price);
