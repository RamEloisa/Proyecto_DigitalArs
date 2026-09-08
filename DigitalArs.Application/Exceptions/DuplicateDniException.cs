namespace DigitalArs.Application.Exceptions;

public sealed class DuplicateDniException : Exception
{
    public DuplicateDniException()
        : base("El DNI ya está registrado.")
    {
    }
}
