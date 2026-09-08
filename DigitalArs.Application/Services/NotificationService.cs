using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Interfaces;

namespace DigitalArs.Application.Services;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetMineAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<bool> MarkReadAsync(
        int userId,
        int notificationId,
        CancellationToken cancellationToken = default);

    Task MarkAllReadAsync(
        int userId,
        CancellationToken cancellationToken = default);
}

public class NotificationService : INotificationService
{
    private const int MaxItems = 50;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<NotificationDto>> GetMineAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var (items, _) = await _unitOfWork.Repository<Notification>()
            .GetPagedProjectedAsync(
                page: 1,
                pageSize: MaxItems,
                selector: n => new NotificationDto(
                    n.ID_Notification,
                    n.Type,
                    n.Title,
                    n.Message,
                    n.Amount,
                    n.IsRead,
                    n.CreatedAt),
                predicate: n => n.ID_User == userId,
                orderBy: n => n.CreatedAt,
                descending: true,
                cancellationToken);

        return items;
    }

    public async Task<bool> MarkReadAsync(
        int userId,
        int notificationId,
        CancellationToken cancellationToken = default)
    {
        var matches = await _unitOfWork.Repository<Notification>()
            .FindAsync(
                n => n.ID_Notification == notificationId && n.ID_User == userId,
                cancellationToken);

        var notification = matches.FirstOrDefault();
        if (notification is null)
        {
            return false;
        }

        if (notification.IsRead)
        {
            return true;
        }

        notification.IsRead = true;
        _unitOfWork.Repository<Notification>().Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task MarkAllReadAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var unread = await _unitOfWork.Repository<Notification>()
            .FindAsync(n => n.ID_User == userId && !n.IsRead, cancellationToken);

        if (unread.Count == 0)
        {
            return;
        }

        foreach (var notification in unread)
        {
            notification.IsRead = true;
            _unitOfWork.Repository<Notification>().Update(notification);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
