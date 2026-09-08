namespace DigitalArs.Application.Exceptions;

public sealed class SourceAccountNotFoundException : Exception
{
    public SourceAccountNotFoundException()
        : base("El usuario no tiene una cuenta.")
    {
    }
}
