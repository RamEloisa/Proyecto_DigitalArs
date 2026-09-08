using MapsterMapper;
using Moq;
using Xunit;

using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Interfaces;
using DigitalArs.Application.Services;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Enum;
using DigitalArs.Application.Abstractions;

namespace DigitalArs.Tests.Deposits;

public class DepositTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Account>> _accountRepositoryMock;
    private readonly Mock<IRepository<Transaction>> _transactionRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRealtimeNotifier> _realtimeNotifierMock;
    private readonly Mock<IRepository<Notification>> _notificationRepositoryMock;
    private readonly AccountService _accountService;

    public DepositTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _accountRepositoryMock = new Mock<IRepository<Account>>();
        _transactionRepositoryMock = new Mock<IRepository<Transaction>>();
        _mapperMock = new Mock<IMapper>();
        _realtimeNotifierMock = new Mock<IRealtimeNotifier>();
        _notificationRepositoryMock = new Mock<IRepository<Notification>>();

        _unitOfWorkMock
            .Setup(u => u.Repository<Account>())
            .Returns(_accountRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(u => u.Repository<Transaction>())
            .Returns(_transactionRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(u => u.Repository<Notification>())
            .Returns(_notificationRepositoryMock.Object);

        _accountService = new AccountService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _realtimeNotifierMock.Object);
    }

    [Fact]
    public async Task DepositAsync_MontoValido_AumentaSaldoYRegistraTransaccion()
    {
        var account = new Account
        {
            ID_Account = 1,
            ID_User = 10,
            Price = 5000
        };

        var dto = new DepositDto(1000);

        var transactionDto = new TransactionDto(
            1,
            account.ID_Account,
            TransactionType.Deposit,
            dto.Amount,
            DateTime.UtcNow);

        _accountRepositoryMock
            .Setup(r => r.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Account, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Account, object>>[]>()))
            .ReturnsAsync(new List<Account> { account });

        _mapperMock
            .Setup(m => m.Map<Transaction, TransactionDto>(
                It.IsAny<Transaction>()))
            .Returns(transactionDto);

        var result = await _accountService.DepositAsync(
            10,
            dto);

        Assert.NotNull(result);
        Assert.Equal(TransactionType.Deposit, result.Type);
        Assert.Equal(1000, result.Amount);
        Assert.Equal(6000, account.Price);

        //verifica que se haya registrado la transaccion
        _transactionRepositoryMock.Verify(
            r => r.AddAsync(
                It.Is<Transaction>(t =>
                    t.ID_Account == account.ID_Account &&
                    t.Type == TransactionType.Deposit &&
                    t.Amount == 1000),
                It.IsAny<CancellationToken>()),
            Times.Once);
        //verifica que se haya actualizado la cuenta
        _accountRepositoryMock.Verify(
            r => r.Update(account),
            Times.Once);
        //verifica que se haya iniciado la transaccion
        _unitOfWorkMock.Verify(
            u => u.BeginTransactionAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
        //verifica el commit de la transaccion
        _unitOfWorkMock.Verify(
            u => u.CommitAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
        //verifica notificacion a usuario
        _realtimeNotifierMock.Verify(
            n => n.NotifyUserAsync(
                account.ID_User,
                It.IsAny<AccountRealtimeEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

    }

    [Fact]
    public async Task DepositAsync_MontoMayorAlMaximo_LanzaInvalidOperationException()
    {
        var account = new Account
        {
            ID_Account = 1,
            ID_User = 10,
            Price = 5000
        };

        // Monto deliberadamente mayor al máximo permitido.
        var dto = new DepositDto(100000000);

        _accountRepositoryMock
            .Setup(r => r.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Account, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Account, object>>[]>()))
            .ReturnsAsync(new List<Account> { account });

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _accountService.DepositAsync(10, dto));

        Assert.Contains(
            "monto máximo por depósito",
            exception.Message,
            StringComparison.OrdinalIgnoreCase);

        Assert.Equal(5000, account.Price);

        _transactionRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Transaction>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _notificationRepositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Notification>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        
        _accountRepositoryMock.Verify(
            r => r.Update(It.IsAny<Account>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.BeginTransactionAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.CommitAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}