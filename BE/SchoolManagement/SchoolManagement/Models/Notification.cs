using System;
namespace SchoolManagement.Models
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }  // Gửi đến user nào
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
