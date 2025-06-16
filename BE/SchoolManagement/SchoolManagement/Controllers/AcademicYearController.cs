using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Helpers;
using SchoolManagement.Hub;
using SchoolManagement.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SchoolManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicYearController : ControllerBase
    {
        private readonly SchoolContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AcademicYearController(SchoolContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet()]
        public async Task<IActionResult> GetRecentYear()
        {
            // Khóa năm học cũ
            var latest = await _context.AcademicYears.OrderByDescending(y => y.CreatedAt).FirstOrDefaultAsync();
            if (latest != null) latest.IsLocked = true;
            return Ok(latest);
        }

        [HttpPost()]
        public async Task<IActionResult> AddAcademicYear()
        {
            // Khóa năm học cũ
            var latest = await _context.AcademicYears.OrderByDescending(y => y.CreatedAt).FirstOrDefaultAsync();
            if (latest != null) latest.IsLocked = true;

            // Tạo năm học mới (auto tăng theo năm cũ)
            var newYear = new AcademicYear
            {
                Year = GenerateNextYear(latest?.Year),
                IsLocked = false,
                CreatedAt = DateTime.Now
            };

            _context.AcademicYears.Add(newYear);
            await _context.SaveChangesAsync();

            var allStudents = await _context.Users
                .Where(u => u.RoleId == RoleType.Student)
                .ToListAsync();

            foreach (var student in allStudents)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = student.Id,
                    Message = $"A new academic year {newYear.Year} has been created."
                });
            }

            await _context.SaveChangesAsync();

            var studentIds = _context.Users
                .Where(u => u.RoleId == RoleType.Student)
                .Select(u => u.Id)
                .ToList();
            foreach (var studentId in studentIds)
            {
                 await NotificationHelper.NotifyAsync(
                     _context,
                     _hubContext,
                     studentId,
                     $"A new academic year '{newYear.Year}' has been created."
                 );
            }
            return Ok(newYear);
        }

        private string GenerateNextYear(string prevYear)
        {
            if (string.IsNullOrEmpty(prevYear)) return "2024-2025";
            var parts = prevYear.Split('-');
            int start = int.Parse(parts[0]) + 1;
            return $"{start}-{start + 1}";
        }
    }
}

