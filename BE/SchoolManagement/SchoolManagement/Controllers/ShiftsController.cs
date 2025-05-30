using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.DTO;
using SchoolManagement.DTO.ShiftDTO;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftsController : ControllerBase
    {
        private readonly SchoolContext _context;

        public ShiftsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: api/Shifts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShiftOutputDTO>> GetShift(int id)
        {
          if (_context.Shifts == null)
          {
              return NotFound();
          }
            var shift = await _context.Shifts
                .Include(s => s.Course)
                .Where(s => s.ShiftId == id)
                .AsSplitQuery()
                .Select(s => new ShiftOutputDTO
                {
                    ShiftId = s.ShiftId,
                    CourseName = s.Course.CourseName,
                    Weekday = s.WeekDay,
                    ShiftCode = s.ShiftCode,
                    MaxQuantity = s.MaxQuantity
                }).FirstOrDefaultAsync();

            if (shift == null)
            {
                return NotFound();
            }

            return Ok(shift);
        }

        // PATCH: api/Shifts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<ActionResult<ShiftOutputDTO>> PatchShift(int id, [FromBody] ShiftAllDTO shiftDTO)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift== null)
            {
                return NotFound();
            }
            if (shiftDTO.ShiftCode.HasValue)
            {
                shift.ShiftCode = (ShiftOfDay)shiftDTO.ShiftCode;
            }
            if (shiftDTO.Weekday.HasValue)
            {
                shift.WeekDay = (WeekDay)shiftDTO.Weekday;
            }
            if (shiftDTO.MaxQuantity.HasValue)
            {
                shift.MaxQuantity = (int)shiftDTO.MaxQuantity;
            }
            if (shiftDTO.CourseId.HasValue)
            {
                shift.CourseId = (int)shiftDTO.CourseId;
            }

            _context.Entry(shift).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShiftExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            shiftDTO.ShiftId = shift.ShiftId;
            return CreatedAtAction("GetShift", new { id = shift.ShiftId }, shift);
        }

        // POST: api/Shifts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        public async Task<ActionResult<ShiftOutputDTO>> PostShift(ShiftInputDTO shiftDTO)
        {
          if (_context.Shifts == null)
          {
              return Problem("Entity set 'SchoolContext.Shifts'  is null.");
          }
            var course = await _context.Courses.Include(c => c.Shifts).FirstOrDefaultAsync(c => c.CourseId == shiftDTO.CourseId);
            Console.WriteLine("=========== FINDDDDD: " + course.CourseName);
            if (course == null)
            {
                return NotFound();
            }

            var overlappingCourses = await _context.Courses
                    .Where(c => c.CourseId != shiftDTO.CourseId &&
                                c.LecturerId == course.LecturerId &&
                                c.StartDate < course.EndDate &&
                                c.EndDate > course.StartDate)
                    .Select(c => c.CourseId)
                    .ToListAsync();
            Console.WriteLine("== === LECTURERID: " + course.LecturerId);
            Console.WriteLine("== === OVERLAP COURSE: " + string.Join(", ", overlappingCourses.Select(c => c)));
            //Console.WriteLine("== === shift: " + string.Join(", ", shifts.Select(s => s.ShiftCode)));

            if (overlappingCourses.Any())
            {
                // Lấy các shifts của các khóa học trùng thời gian
                var overlappingShifts = await _context.Shifts
                    .Where(s => overlappingCourses.Contains(s.CourseId))
                    .Select(s => new { s.WeekDay, s.ShiftCode })
                    .ToListAsync();

                var shiftKeySet = new HashSet<string>(overlappingShifts.Select(s => $"{(int)s.WeekDay}-{(int)s.ShiftCode}"));
                Console.WriteLine("== === shift key set: " + string.Join(", ", shiftKeySet.Select(s => s)));
                var key = $"{(int)shiftDTO.WeekDay}-{(int)shiftDTO.ShiftOfDay}";
                Console.WriteLine("== KEY NÈ: " + key);
                if (shiftKeySet.Contains(key))
                {
                    return BadRequest($"Shift at: {shiftDTO.WeekDay} with time: {shiftDTO.ShiftOfDay} exists in another course of this lecturer.");
                }
            }

            //var shift = await _context.Shifts
            //    .Include(s => s.Course)
            //    .Select(s => new Shift
            //    {
            //        ShiftCode = shiftDTO.ShiftOfDay,
            //        CourseId = shiftDTO.CourseId,
            //        WeekDay = shiftDTO.WeekDay,
            //        MaxQuantity = shiftDTO.maxQuantity ?? 25
            //    }).FirstOrDefaultAsync();


            //_context.Shifts.Add(shift);
            //await _context.SaveChangesAsync();

            //var outputShift = new ShiftOutputDTO
            //{
            //    ShiftId = shift.ShiftId,
            //    Weekday = shift.WeekDay,
            //    CourseName = shift.Course?.CourseName,
            //    MaxQuantity = shift.MaxQuantity,
            //    ShiftCode = shift.ShiftCode,
            //};

            //return CreatedAtAction("GetShift", new { id = shift.ShiftId }, outputShift);
            return Ok("PASS ALL");
        }

        // DELETE: api/Shifts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShift(int id)
        {
            if (_context.Shifts == null)
            {
                return NotFound();
            }
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null)
            {
                return NotFound();
            }

            // Kiểm tra số lượng shifts thuộc courseId đang muốn xóa
            var courseId = shift.CourseId; 
            var shiftCount = await _context.Shifts.CountAsync(s => s.CourseId == courseId);

            // Nếu số lượng shifts còn lại (bao gồm cả shift đang muốn xóa) là 2 thì không cho phép xóa
            if (shiftCount <= 2)
            {
                return BadRequest("Cannot delete this shift. At least 2 shifts must remain for this course.");
            }

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShiftExists(int id)
        {
            return (_context.Shifts?.Any(e => e.ShiftId == id)).GetValueOrDefault();
        }
    }
}
