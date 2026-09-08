using DigitalArs.Application.DTOs;
using DigitalArs.Application.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace DigitalArs.Application.Validators;

public sealed class CreateFixedTermDepositDtoValidator : AbstractValidator<CreateFixedTermDepositDto>
{
    public CreateFixedTermDepositDtoValidator(IOptions<FixedTermDepositSettings> options)
    {
        var settings = options.Value;

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(settings.MinAmount)
            .WithMessage($"El monto mínimo es {settings.MinAmount}.")
            .PrecisionScale(18, 2, ignoreTrailingZeros: true)
            .WithMessage("El monto admite como máximo 2 decimales.");

        RuleFor(x => x.TermDays)
            .GreaterThanOrEqualTo(settings.MinTermDays)
            .WithMessage($"El plazo mínimo es de {settings.MinTermDays} días.");
    }
}
