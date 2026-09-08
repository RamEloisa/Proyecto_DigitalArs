namespace DigitalArs.Application.Exceptions;

public sealed class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException()
        : base("Saldo insuficiente.")
    {
    }
}
