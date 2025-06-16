using System;
using Microsoft.AspNetCore.SignalR;
using SchoolManagement.Hub;
using SchoolManagement.Models;

namespace SchoolManagement.Helpers
{
	public class NotificationHelper
	{
        public static void Notify(SchoolContext context, string userId, string message)
        {
            context.Notifications.Add(new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });
        }

        public static async Task NotifyAsync(
            SchoolContext context,
            IHubContext<NotificationHub> hub,
            string userId,
            string message
        )
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();

            await hub.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                notification.Id,
                notification.Message,
                notification.CreatedAt
            });
        }
    }
}

