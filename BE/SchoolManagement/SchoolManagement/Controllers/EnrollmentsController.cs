using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.DTO.EnrollmentDTO;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly SchoolContext _context;

        public EnrollmentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: api/Enrollments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollments()
        {
          if (_context.Enrollments == null)
          {
              return NotFound();
          }
            return await _context.Enrollments.ToListAsync();
        }

        // GET: api/Enrollments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Enrollment>> GetEnrollment(int id)
        {
          if (_context.Enrollments == null)
          {
              return NotFound();
          }
            var enrollment = await _context.Enrollments.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return enrollment;
        }

        // PUT: api/Enrollments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnrollment(string id, Enrollment enrollment)
        {
            if (id != enrollment.UserId)
            {
                return BadRequest();
            }

            _context.Entry(enrollment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnrollmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Enrollments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostEnrollment(EnrollmentInputPostDTO enrollmentDTO, CancellationToken cancellationToken = default)
        {
          if (_context.Enrollments == null)
          {
              return Problem("Entity set 'SchoolContext.Enrollments'  is null.");
          }
            foreach (var item in enrollmentDTO.ShiftId)
            {
                // Kiểm tra điều kiện là trong enrollmentDTO có thằng shift nào có userId giống vs ng dùng hiện tại mà trùng shiftOfDay và WeekDay không, nếu ko tồn tại thằng nào thì mới thêm các shift đó vào enrollment. 
                // Sau đó kiểm tra số lượng enrollment mà có shiftId đã vượt qua maxQuantity của shift đó chưa => nếu chưa thì mới add, nếu có bất kỳ cái nào đã vượt maxQuantity thì return BadRequest()
            }
            //var enrollment = new Enrollment
            //{
            //    UserId = enrollmentDTO.UserId,
            //    ShiftId = enrollmentDTO.ShiftId,
            //    TimeJoined = DateTime.Now,
            //};

            //_context.Enrollments.Add(enrollment);
            //await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        // DELETE: api/Enrollments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            if (_context.Enrollments == null)
            {
                return NotFound();
            }
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnrollmentExists(string id)
        {
            return (_context.Enrollments?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
