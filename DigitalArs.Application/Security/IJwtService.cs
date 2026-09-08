namespace DigitalArs.Application.Security;

public interface IJwtService
{
    string GenerateToken(
        int userId,
        string email,
        string role);
}