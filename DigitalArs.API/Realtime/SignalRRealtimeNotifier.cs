using DigitalArs.API.Hubs;
using DigitalArs.Application.Abstractions;
using DigitalArs.Application.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace DigitalArs.API.Realtime;

public sealed class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<AccountHub> _hub;

    public SignalRRealtimeNotifier(IHubContext<AccountHub> hub)
    {
        _hub = hub;
    }

    public async Task NotifyUserAsync(
        int userId,
        AccountRealtimeEvent evt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hub.Clients
                .Group(AccountHub.GroupName(userId))
                .SendAsync("AccountUpdated", evt, cancellationToken);
        }
        catch
        {
            // El movimiento ya está persistido; el cliente lo verá al recargar notificaciones.
        }
    }
}
