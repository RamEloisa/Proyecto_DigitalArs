using DigitalArs.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DigitalArs.API.Hubs;

[Authorize]
public class AccountHub : Hub
{
    public static string GroupName(int userId) => $"user-{userId}";

    public override async Task OnConnectedAsync()
    {
        var userId = CurrentUserHelper.GetUserId(Context.User!);
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(userId));
        await base.OnConnectedAsync();
    }
}
