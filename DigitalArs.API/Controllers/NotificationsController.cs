using DigitalArs.Application.DTOs;
using DigitalArs.Application.Security;
using DigitalArs.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalArs.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Tags("Notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notifications;

    public NotificationsController(INotificationService notifications)
    {
        _notifications = notifications;
    }

    [HttpGet("me")]
    [EndpointSummary("Lista las notificaciones recientes del usuario autenticado")]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetMine(
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserHelper.GetUserId(User);
        var items = await _notifications.GetMineAsync(userId, cancellationToken);
        return Ok(items);
    }

    [HttpPatch("{id:int}/read")]
    [EndpointSummary("Marca una notificación como leída")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead(int id, CancellationToken cancellationToken)
    {
        var userId = CurrentUserHelper.GetUserId(User);
        var updated = await _notifications.MarkReadAsync(userId, id, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpPost("read-all")]
    [EndpointSummary("Marca todas las notificaciones del usuario como leídas")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        var userId = CurrentUserHelper.GetUserId(User);
        await _notifications.MarkAllReadAsync(userId, cancellationToken);
        return NoContent();
    }
}
