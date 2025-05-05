using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.DTO;
using SchoolManagement.DTO.ShiftDTO;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftsController : ControllerBase
    {
        private readonly SchoolContext _context;

        public ShiftsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: api/Shifts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftAllDTO>>> GetShifts(int pageNumber = 1, int pageSize = 10)
        {
            var shifts = await _context.Shifts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Course)
                .AsSplitQuery()
                .Select(s => new ShiftAllDTO
                {
                    ShiftId = s.ShiftId,
                    CourseId = s.CourseId,
                    CourseName = s.Course.CourseName,
                    Weekday = s.WeekDay,
                    ShiftCode = s.ShiftCode,
                    MaxQuantity = s.MaxQuantity
                }).ToListAsync();

            return Ok(shifts);
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
        [HttpPost]
        public async Task<ActionResult<ShiftOutputDTO>> PostShift(ShiftInputDTO shiftDTO)
        {
          if (_context.Shifts == null)
          {
              return Problem("Entity set 'SchoolContext.Shifts'  is null.");
          }
            var shift = await _context.Shifts
                .Include(s => s.Course)
                .Select(s => new Shift
                {
                    ShiftCode = shiftDTO.ShiftOfDay,
                    CourseId = shiftDTO.CourseId,
                    WeekDay = shiftDTO.WeekDay,
                    MaxQuantity = 30 //default number
                }).FirstOrDefaultAsync();


            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            var outputShift = new ShiftOutputDTO
            {
                ShiftId = shift.ShiftId,
                Weekday = shift.WeekDay,
                CourseName = shift.Course.CourseName,
                MaxQuantity = shift.MaxQuantity,
                ShiftCode = shift.ShiftCode,
            };

            //shiftDTO.CourseName = shift.Course.CourseName;
            return CreatedAtAction("GetShift", new { id = shift.ShiftId }, outputShift);
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
