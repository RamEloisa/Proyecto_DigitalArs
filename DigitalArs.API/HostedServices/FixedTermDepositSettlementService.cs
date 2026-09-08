using DigitalArs.Application.Services;

namespace DigitalArs.API.HostedServices;

public sealed class FixedTermDepositSettlementService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(60);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FixedTermDepositSettlementService> _logger;

    public FixedTermDepositSettlementService(
        IServiceScopeFactory scopeFactory,
        ILogger<FixedTermDepositSettlementService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var deposits = scope.ServiceProvider.GetRequiredService<IFixedTermDepositService>();
                await deposits.SettleMaturedAsync(cancellationToken: stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al liquidar plazos fijos vencidos.");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
