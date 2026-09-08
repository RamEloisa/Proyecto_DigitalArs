namespace DigitalArs.Application.Exceptions;

public sealed class SelfTransferException : Exception
{
    public SelfTransferException()
        : base("No se permite transferir a la propia cuenta.")
    {
    }
}
