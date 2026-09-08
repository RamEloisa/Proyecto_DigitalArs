using DigitalArs.Application.DTOs;

namespace DigitalArs.Application.Abstractions;

public interface IRealtimeNotifier
{
    Task NotifyUserAsync(
        int userId,
        AccountRealtimeEvent evt,
        CancellationToken cancellationToken = default);
}
