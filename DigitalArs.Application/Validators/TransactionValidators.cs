using DigitalArs.Application.DTOs;
using FluentValidation;

namespace DigitalArs.Application.Validators;

public sealed class TransactionQueryDtoValidator : AbstractValidator<TransactionQueryDto>
{
    public TransactionQueryDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("La página debe ser mayor a 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("El tamaño de página debe estar entre 1 y 100.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("El tipo de transacción no es válido.")
            .When(x => x.Type.HasValue);

        RuleFor(x => x.MinAmount)
            .GreaterThanOrEqualTo(0).WithMessage("El monto mínimo no puede ser negativo.")
            .When(x => x.MinAmount.HasValue);

        RuleFor(x => x.MaxAmount)
            .GreaterThanOrEqualTo(0).WithMessage("El monto máximo no puede ser negativo.")
            .When(x => x.MaxAmount.HasValue);

        RuleFor(x => x)
            .Must(x => x.FromDate is null || x.ToDate is null || x.FromDate <= x.ToDate)
            .WithMessage("La fecha desde no puede ser posterior a la fecha hasta.")
            .OverridePropertyName("FromDate");

        RuleFor(x => x)
            .Must(x => x.MinAmount is null || x.MaxAmount is null || x.MinAmount <= x.MaxAmount)
            .WithMessage("El monto mínimo no puede ser mayor al monto máximo.")
            .OverridePropertyName("MinAmount");
    }
}

public sealed class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionDtoValidator()
    {
        RuleFor(x => x.AccountId)
            .GreaterThan(0).WithMessage("La cuenta es obligatoria.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("El tipo de transacción no es válido.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0.")
            .PrecisionScale(18, 2, ignoreTrailingZeros: true)
            .WithMessage("El monto admite como máximo 2 decimales.");
    }
}

public sealed class TransferDtoValidator : AbstractValidator<TransferDto>
{
    public TransferDtoValidator()
    {
        RuleFor(x => x.DestinationAccountId)
            .GreaterThan(0).WithMessage("La cuenta destino es obligatoria.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0.")
            .PrecisionScale(18, 2, ignoreTrailingZeros: true)
            .WithMessage("El monto admite como máximo 2 decimales.");
    }
}
