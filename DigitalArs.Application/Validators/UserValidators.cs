using DigitalArs.Application.DTOs;
using FluentValidation;

namespace DigitalArs.Application.Validators;

public sealed class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre completo no puede superar los 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El email no puede superar los 150 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .MaximumLength(100).WithMessage("La contraseña no puede superar los 100 caracteres.");

        RuleFor(x => x.Dni)
            .NotEmpty().WithMessage("El DNI es obligatorio.")
            .MaximumLength(20).WithMessage("El DNI no puede superar los 20 caracteres.")
            .Matches(@"^\d+$").WithMessage("El DNI solo puede contener dígitos.");

        RuleFor(x => x.Alias)
            .NotEmpty().WithMessage("El alias es obligatorio.")
            .MaximumLength(50).WithMessage("El alias no puede superar los 50 caracteres.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("El rol es obligatorio.");
    }
}

public sealed class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre completo no puede superar los 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El email no puede superar los 150 caracteres.");

        RuleFor(x => x.Dni)
            .NotEmpty().WithMessage("El DNI es obligatorio.")
            .MaximumLength(20).WithMessage("El DNI no puede superar los 20 caracteres.")
            .Matches(@"^\d+$").WithMessage("El DNI solo puede contener dígitos.");

        RuleFor(x => x.Alias)
            .NotEmpty().WithMessage("El alias es obligatorio.")
            .MaximumLength(50).WithMessage("El alias no puede superar los 50 caracteres.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("El rol es obligatorio.");
    }
}

public sealed class UserQueryDtoValidator : AbstractValidator<UserQueryDto>
{
    public UserQueryDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("La página debe ser mayor a 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("El tamaño de página debe estar entre 1 y 100.");
    }
}
