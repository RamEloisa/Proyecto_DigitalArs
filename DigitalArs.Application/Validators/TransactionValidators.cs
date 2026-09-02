using DigitalArs.Application.DTOs;
using FluentValidation;

namespace DigitalArs.Application.Validators;

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
