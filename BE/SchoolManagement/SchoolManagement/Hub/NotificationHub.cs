namespace SchoolManagement.Hub;
using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    // Optional: override OnConnectedAsync to verify identity
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"User connected: {Context.UserIdentifier}");
        return base.OnConnectedAsync();
    }
    public async Task RegisterUser(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }
}
