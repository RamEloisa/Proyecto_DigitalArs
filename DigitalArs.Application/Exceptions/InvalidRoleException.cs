namespace DigitalArs.Application.Exceptions;

public sealed class InvalidRoleException : Exception
{
    public InvalidRoleException()
        : base("El rol no existe.")
    {
    }
}
