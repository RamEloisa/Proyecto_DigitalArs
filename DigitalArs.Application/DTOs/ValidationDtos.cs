namespace DigitalArs.Application.DTOs;

/// Error de validación de un campo del payload.
public record ValidationErrorDto(string Field, string Message);

/// Respuesta 400: lista de errores de validación (nunca se mezclan datos de dominio).
public record ValidationProblemDto(int Status, string Message, IReadOnlyList<ValidationErrorDto> Errors, string TraceId);
