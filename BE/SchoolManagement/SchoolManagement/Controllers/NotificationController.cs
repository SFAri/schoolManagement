using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly SchoolContext _context;

        public NotificationController(SchoolContext context)
        {
            _context = context;
        }

        // GET: api/Notification?userId=abc&page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetNotifications(string userId, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId is required.");

            var query = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt);

            var total = await query.CountAsync();
            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                total,
                page,
                pageSize,
                data = notifications
            });
        }

        // PUT: api/Notification/{id}/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Notification/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Notification/unread-count?userId=abc
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId is required.");

            var count = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

            return Ok(new { unreadCount = count });
        }
    }
}

