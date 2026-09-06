using DigitalArs.Domain.Enum;

namespace DigitalArs.Application.DTOs;

public record CreateFixedTermDepositDto(decimal Amount, int TermDays);

public record FixedTermDepositDto(
    int Id,
    decimal Amount,
    decimal AnnualRate,
    int TermDays,
    decimal InterestAmount,
    decimal FinalAmount,
    DateTime CreatedAt,
    DateTime MaturityDate,
    FixedTermDepositStatus Status);

public record FixedTermDepositMeDto(
    decimal Amount,
    decimal AnnualRate,
    DateTime MaturityDate,
    FixedTermDepositStatus Status);
