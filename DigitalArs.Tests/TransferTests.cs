//transferencia exitosa
//saldo insuficiente
//destino inexistente
//autotransferencia
//rollback
using MapsterMapper;
using Moq;
using Xunit;
using System.Linq.Expressions;

using DigitalArs.Application.DTOs;
using DigitalArs.Application.Exceptions;
using DigitalArs.Domain.Interfaces;
using DigitalArs.Application.Services;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Enum;

namespace DigitalArs.Tests.Transfers;

public class TransferTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Account>> _accountRepositoryMock;
    private readonly Mock<IRepository<Transaction>> _transactionRepositoryMock;
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly TransactionService _transactionService;

    public TransferTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _accountRepositoryMock = new Mock<IRepository<Account>>();
        _transactionRepositoryMock = new Mock<IRepository<Transaction>>();
        _userRepositoryMock = new Mock<IRepository<User>>();
        _mapperMock = new Mock<IMapper>();

        _unitOfWorkMock
            .Setup(u => u.Repository<Account>())
            .Returns(_accountRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(u => u.Repository<Transaction>())
            .Returns(_transactionRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(u => u.Repository<User>())
            .Returns(_userRepositoryMock.Object);

        _transactionService = new TransactionService(
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task TransferAsync_TransferenciaValida_ActualizaSaldosYHaceCommit()
    {
        var source = new Account
        {
            ID_Account = 1,
            ID_User = 10,
            Price = 5000
        };

        var destination = new Account
        {
            ID_Account = 2,
            ID_User = 20,
            Price = 2000,
            User = new User
            {
                ID_User = 20,
                IsActive = true
            }
        };

        var dto = new TransferDto(
            DestinationAccountId: 2,
            Amount: 1000);

        var transferOutDto = new TransactionDto(
            1,
            1,
            TransactionType.Transfer_Out,
            1000,
            DateTime.UtcNow);

        var transferInDto = new TransactionDto(
            2,
            2,
            TransactionType.Transfer_In,
            1000,
            DateTime.UtcNow);

        _accountRepositoryMock
            .Setup(r => r.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Account, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Account, object>>[]>()))
            .ReturnsAsync(
                (System.Linq.Expressions.Expression<Func<Account, bool>> predicate,
                 CancellationToken cancellationToken,
                 System.Linq.Expressions.Expression<Func<Account, object>>[] includes) =>
                {
                    // Identificamos qué cuenta está buscando el servicio.
                    var compiled = predicate.Compile();

                    var accounts = new List<Account>
                    {
                        source,
                        destination
                    };

                    return accounts
                        .Where(compiled)
                        .ToList();
                });

        _mapperMock
            .Setup(m => m.Map<Transaction, TransactionDto>(
                It.Is<Transaction>(t => t.Type == TransactionType.Transfer_Out)))
            .Returns(transferOutDto);

        _mapperMock
            .Setup(m => m.Map<Transaction, TransactionDto>(
                It.Is<Transaction>(t => t.Type == TransactionType.Transfer_In)))
            .Returns(transferInDto);

        var result = await _transactionService.TransferAsync(
            sourceUserId: 10,
            dto);

        Assert.NotNull(result);

        Assert.Equal(4000, source.Price);
        Assert.Equal(3000, destination.Price);

        Assert.Equal(TransactionType.Transfer_Out, result.TransferOut.Type);
        Assert.Equal(TransactionType.Transfer_In, result.TransferIn.Type);

        _accountRepositoryMock.Verify(
            r => r.Update(source),
            Times.Once);

        _accountRepositoryMock.Verify(
            r => r.Update(destination),
            Times.Once);

        _transactionRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Transaction>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        _unitOfWorkMock.Verify(
            u => u.BeginTransactionAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.RollbackAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task TransferAsync_SaldoInsuficiente_LanzaException()
    {
        var source = new Account
        {
            ID_Account = 1,
            ID_User = 10,
            Price = 500
        };

        var destination = new Account
        {
            ID_Account = 2,
            ID_User = 20,
            Price = 2000,
            User = new User
            {
                ID_User = 20,
                IsActive = true
            }
        };

        var dto = new TransferDto(2, 1000);

        ConfigurarBusquedaDeCuentas(source, destination);

        var exception = await Assert.ThrowsAsync<InsufficientBalanceException>(
            () => _transactionService.TransferAsync(10, dto));

        Assert.Equal("Saldo insuficiente.", exception.Message);

        Assert.Equal(500, source.Price);
        Assert.Equal(2000, destination.Price);

        _transactionRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Transaction>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.RollbackAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task TransferAsync_DestinoInexistente_LanzaException()
    {
        var source = new Account
        {
            ID_Account = 1,
            ID_User = 10,
            Price = 5000
        };

        ConfigurarBusquedaDeCuentas(source);

        var dto = new TransferDto(999, 1000);

        var exception = await Assert.ThrowsAsync<DestinationAccountNotFoundException>(
            () => _transactionService.TransferAsync(10, dto));

        Assert.Equal(
            "La cuenta destino no existe o no está activa.",
            exception.Message);

        Assert.Equal(5000, source.Price);

        _transactionRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Transaction>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.RollbackAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task TransferAsync_MismaCuenta_LanzaException()
    {
        // Arrange
        var account = new Account
        {
            ID_Account = 1,
            ID_User = 10,
            Price = 5000,
            User = new User
            {
                ID_User = 10,
                IsActive = true
            }
        };

        _accountRepositoryMock
            .Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<Account, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Account, object>>[]>()))
            .ReturnsAsync(
                (Expression<Func<Account, bool>> predicate,
                 CancellationToken cancellationToken,
                 Expression<Func<Account, object>>[] includes) =>
                {
                    var compiled = predicate.Compile();

                    return new List<Account> { account }
                        .Where(compiled)
                        .ToList();
                });

        var dto = new TransferDto(
            DestinationAccountId: 1,
            Amount: 1000);

        // Act & Assert
        await Assert.ThrowsAsync<SelfTransferException>(
            () => _transactionService.TransferAsync(10, dto));

        // Verificaciones
        Assert.Equal(5000, account.Price);

        _transactionRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Transaction>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.RollbackAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task TransferAsync_ErrorAlAgregarTransaccion_EjecutaRollback()
    {
        var source = new Account
        {
            ID_Account = 1,
            ID_User = 10,
            Price = 5000
        };

        var destination = new Account
        {
            ID_Account = 2,
            ID_User = 20,
            Price = 2000,
            User = new User
            {
                ID_User = 20,
                IsActive = true
            }
        };

        var dto = new TransferDto(2, 1000);

        ConfigurarBusquedaDeCuentas(source, destination);

        _transactionRepositoryMock
            .Setup(r => r.AddAsync(
                It.IsAny<Transaction>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error de prueba"));

        await Assert.ThrowsAsync<Exception>(
            () => _transactionService.TransferAsync(10, dto));

        _unitOfWorkMock.Verify(
            u => u.BeginTransactionAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.RollbackAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void ConfigurarBusquedaDeCuentas(
        Account source,
        Account? destination = null)
    {
        _accountRepositoryMock
            .Setup(r => r.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Account, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Account, object>>[]>()))
            .ReturnsAsync(
                (System.Linq.Expressions.Expression<Func<Account, bool>> predicate,
                 CancellationToken cancellationToken,
                 System.Linq.Expressions.Expression<Func<Account, object>>[] includes) =>
                {
                    var compiled = predicate.Compile();

                    var accounts = new List<Account> { source };

                    if (destination is not null)
                    {
                        accounts.Add(destination);
                    }

                    return accounts
                        .Where(compiled)
                        .ToList();
                });
    }
}