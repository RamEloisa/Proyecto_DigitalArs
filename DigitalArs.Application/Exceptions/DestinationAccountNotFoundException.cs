namespace DigitalArs.Application.Exceptions;

public sealed class DestinationAccountNotFoundException : Exception
{
    public DestinationAccountNotFoundException()
        : base("La cuenta destino no existe o no está activa.")
    {
    }
}
