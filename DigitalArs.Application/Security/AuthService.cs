using DigitalArs.Application.DTOs.Auth;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace DigitalArs.Application.Security;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request)
    {
        var users = await _unitOfWork
            .Repository<User>()
            .FindAsync(
                u => u.Email == request.Email,
                default,
                u => u.Role);

        var user = users.FirstOrDefault();

        // No revelamos si el email existe o si la contraseña es incorrecta.
        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Credenciales inválidas.");
        }

         if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Credenciales inválidas.");
        }

        var passwordIsValid = _passwordHasher.Verify(
            request.Password,
            user.Password_Hasheada);

        if (!passwordIsValid)
        {
            throw new UnauthorizedAccessException(
                "Credenciales inválidas.");
        }

        var token = _jwtService.GenerateToken(
            user.ID_User,
            user.Email,
            user.Role.Name);

        var expiresAt = DateTime.UtcNow.AddMinutes(
            _jwtSettings.ExpirationMinutes);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt
        };
    }
}