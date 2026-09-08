using Microsoft.Extensions.Options;
using Moq;
using Xunit;

using DigitalArs.Application.Security;
using DigitalArs.Domain.Interfaces;
using DigitalArs.Domain.Entities;
using DigitalArs.Application.DTOs;

namespace DigitalArs.Tests;

public class LoginTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<User>> _repositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtService> _jwtServiceMock;

    private readonly AuthService _authService;

    public LoginTests()
    {
        // Creamos los mocks
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IRepository<User>>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtServiceMock = new Mock<IJwtService>();

        // Configuración falsa de JWT para el test
        var jwtSettings = Options.Create(new JwtSettings
        {
            ExpirationMinutes = 60
        });

        // Cuando AuthService pida el repositorio de User,
        // le devolvemos nuestro repositorio falso.
        _unitOfWorkMock
            .Setup(u => u.Repository<User>())
            .Returns(_repositoryMock.Object);

        // Creamos el servicio que vamos a probar
        _authService = new AuthService(
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            jwtSettings);
    }

    [Fact]
    public async Task LoginAsync_CredencialesValidas_RetornaToken()
    {
        // ARRANGE
        var user = new User
        {
            ID_User = 1,
            Full_Name = "Juan Perez",
            Email = "juan.perez@digitalars.com",
            Password_Hasheada = "hash-de-prueba",
            DNI = "35222333",
            Alias = "juan.perez",
            IsActive = true,
            ID_Role = 2,
            Role = new Role
            {
                ID_Role = 2,
                Name = "User"
            }
        };

        var request = new LoginRequestDto
        {
            Email = "juan.perez@digitalars.com",
            Password = "User123!"
        };

        // El repositorio devuelve el usuario
        _repositoryMock
            .Setup(r => r.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(new List<User> { user });

        // La contraseña es correcta
        _passwordHasherMock
            .Setup(p => p.Verify(
                request.Password,
                user.Password_Hasheada))
            .Returns(true);

        // El JWT devuelve un token de prueba
        _jwtServiceMock
            .Setup(j => j.GenerateToken(
                user.ID_User,
                user.Email,
                user.Role.Name))
            .Returns("token-de-prueba");

        // ACT
        var result = await _authService.LoginAsync(request);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal("token-de-prueba", result.Token);

        // Verificamos que se haya generado el JWT
        _jwtServiceMock.Verify(
            j => j.GenerateToken(
                user.ID_User,
                user.Email,
                user.Role.Name),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ContrasenaIncorrecta_LanzaUnauthorizedAccessException()
    {
        // ARRANGE
        var user = new User
        {
            ID_User = 1,
            Full_Name = "Juan Perez",
            Email = "juan.perez@digitalars.com",
            Password_Hasheada = "hash-de-prueba",
            DNI = "35222333",
            Alias = "juan.perez",
            IsActive = true,
            ID_Role = 2,
            Role = new Role
            {
                ID_Role = 2,
                Name = "User"
            }
        };

        var request = new LoginRequestDto
        {
            Email = "juan.perez@digitalars.com",
            Password = "PasswordIncorrecta"
        };

        // El repositorio encuentra al usuario
        _repositoryMock
            .Setup(r => r.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(new List<User> { user });

        // La contraseña es incorrecta
        _passwordHasherMock
            .Setup(p => p.Verify(
                request.Password,
                user.Password_Hasheada))
            .Returns(false);

        // ACT + ASSERT

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(request));

        Assert.Equal("Credenciales inválidas.", exception.Message);

        // Verificamos que NO se haya generado un JWT
        _jwtServiceMock.Verify(
            j => j.GenerateToken(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
                Times.Never);
    }


    [Fact]
    public async Task LoginAsync_UsuarioInactivo_LanzaUnauthorizedAccessException()
    {
        // ARRANGE
        var user = new User
        {
            ID_User = 1,
            Full_Name = "Juan Perez",
            Email = "juan.perez@digitalars.com",
            Password_Hasheada = "hash-de-prueba",
            DNI = "35222333",
            Alias = "juan.perez",
            IsActive = false,
            ID_Role = 2,
            Role = new Role
            {
                ID_Role = 2,
                Name = "User"
            }
        };

        var request = new LoginRequestDto
        {
            Email = "juan.perez@digitalars.com",
            Password = "User123!"
        };

        // El repositorio encuentra al usuario
        _repositoryMock
            .Setup(r => r.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(new List<User> { user });

        // ACT + ASSERT
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(request));

        // Verificamos el mensaje
        Assert.Equal("Credenciales inválidas.", exception.Message);

        // La contraseña NO debería haberse verificado
        _passwordHasherMock.Verify(
            p => p.Verify(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        // Tampoco debería haberse generado un JWT
        _jwtServiceMock.Verify(
            j => j.GenerateToken(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

}