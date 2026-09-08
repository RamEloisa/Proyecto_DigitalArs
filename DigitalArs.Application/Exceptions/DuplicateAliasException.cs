namespace DigitalArs.Application.Exceptions;

public sealed class DuplicateAliasException : Exception
{
    public DuplicateAliasException()
        : base("El alias ya está registrado.")
    {
    }
}
