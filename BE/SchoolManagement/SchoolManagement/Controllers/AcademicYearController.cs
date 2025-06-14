using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SchoolManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicYearController : ControllerBase
    {
        private readonly SchoolContext _context;

        public AcademicYearController(SchoolContext context)
        {
            _context = context;
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

