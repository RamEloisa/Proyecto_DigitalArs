namespace DigitalArs.Application.Exceptions;

public sealed class DuplicateEmailException : Exception
{
    public DuplicateEmailException()
        : base("El email ya está registrado.")
    {
    }
}
