using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.DTO;
using SchoolManagement.DTO.CourseDTO;
using SchoolManagement.DTO.ShiftDTO;
using SchoolManagement.DTO.UserDTO;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseOutputGetDTO>>> GetCourses(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
          if (_context.Courses == null)
          {
              return NotFound();
          }
            var totalCourses = await _context.Courses.CountAsync();
            var courses = await _context.Courses
                  .OrderBy(c => c.CourseId)
                  //.Skip((pageNumber - 1) * pageSize)
                  //.Take(pageSize)
                  .Include(c => c.Lecturer) 
                  .Include(c => c.Shifts)
                  //.Include(c => c.Scores)     
                  .AsSplitQuery()
                  .Select(c => new CourseOutputGetDTO
                  {
                      CourseId = c.CourseId,
                      CourseName = c.CourseName,
                      StartDate = c.StartDate,
                      EndDate = c.EndDate,
                      Lecturer = new LecturerOutputDTO
                      {
                          Email = c.Lecturer.Email,
                          LecturerId = c.LecturerId,
                          LecturerName = c.Lecturer.FirstName + ' ' + c.Lecturer.LastName
                      },
                      Shifts = c.Shifts.Select(s => new ShiftOutputCourseDTO
                      {
                          ShiftId = s.ShiftId,
                          WeekDay = s.WeekDay,
                          ShiftOfDay = s.ShiftCode
                      })
                      //Scores = c.Scores.Select(sc => new ScoreDTO
                      //{
                      //    StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                      //    ScoreNum = sc.ScoreNum // Thay đổi nếu cần
                      //})
                      .ToList() // Lấy danh sách điểm số
                  })
                  .ToListAsync(cancellationToken);

            return Ok(new { data = courses, totalCount = totalCourses });
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseOutputGetDTO>> GetCourse(int id, CancellationToken cancellationToken = default)
        {
          if (_context.Courses == null)
          {
              return NotFound();
          }
            var course = await _context.Courses
            .Include(c => c.Lecturer) // Bao gồm thông tin giảng viên
            .Include(c => c.Shifts)    // Bao gồm các shift liên quan
            .Include(c => c.Scores)     // Bao gồm các điểm số
            .Where(c => c.CourseId == id) // Tìm khóa học theo ID
            .AsSplitQuery()
            .Select(c => new CourseOutputGetDTO
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Lecturer= new LecturerOutputDTO
                {
                    Email = c.Lecturer.Email,
                    LecturerId = c.LecturerId,
                    LecturerName = c.Lecturer.FirstName + ' ' + c.Lecturer.LastName
                },
                Shifts = c.Shifts.Select(s => new ShiftOutputCourseDTO
                {
                    ShiftId = s.ShiftId,
                    WeekDay = s.WeekDay,
                    ShiftOfDay = s.ShiftCode,
                    MaxQuantity = s.MaxQuantity
                }).ToList(), // Chọn các thuộc tính mà bạn muốn từ Shift

                //Scores = c.Scores.Select(sc => new ScoreDTO
                //{
                //    StudentName = sc.User.FirstName + ' ' + sc.User.LastName, // Giả định rằng Score có thuộc tính StudentId
                //    ScoreNum = sc.ScoreNum, // Giả định rằng Score có thuộc tính ScoreNum
                //    CreatedAt = sc.CreatedAt,
                //    UpdatedAt = sc.LastUpdatedAt
                //}).ToList() // Chọn các thuộc tính mà bạn muốn từ Score
            })
            .FirstOrDefaultAsync(cancellationToken); // Lấy một khóa học đầu tiên (hoặc null)

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        // PATCH: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<CourseOutputGetDTO>> PatchCourse([FromRoute]int id, [FromBody] CoursePatchInputDTO courseDTO, CancellationToken cancellationToken = default)
        {
            if (courseDTO == null)
            {
                return BadRequest();
            }
            var course = await _context.Courses.FindAsync(id);
            Console.WriteLine("=========== FINDDDDD: ", course);
            if (course == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin khóa học
            if (courseDTO.CourseName != null)
            {
                course.CourseName = courseDTO.CourseName;
            }
            if (courseDTO.StartDate != null)
            {
                course.StartDate = (DateTime)courseDTO.StartDate;
            }
            if (courseDTO.EndDate.HasValue)
            {
                course.EndDate = (DateTime)courseDTO.EndDate;
            }
            if (courseDTO.LecturerId!=null)
            {
                course.LecturerId = courseDTO.LecturerId;
            }
            if (courseDTO.Shifts != null)
            {
                foreach (var item in _context.Shifts.Where(s => s.CourseId == course.CourseId ))
                {
                    _context.Shifts.Remove(item);
                }
                await _context.SaveChangesAsync(cancellationToken);
                course.Shifts = courseDTO.Shifts.Select(s => new Shift
                {
                    WeekDay = s.WeekDay,
                    ShiftCode = s.ShiftCode,
                    MaxQuantity = s.MaxQuantity,
                    CourseId = course.CourseId
                }).ToList();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var courseOutput = await GetCourse(course.CourseId, cancellationToken); // Gọi tới phương thức GetCourse
            if (courseOutput.Result is NotFoundResult) // Kiểm tra nếu không tìm thấy
            {
                return NotFound(); // Trả về không tìm thấy
            }

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, courseOutput.Value);
        }

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CourseOutputGetDTO>> PostCourse(CoursePostInputDTO courseDTO, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("CourseDTO: " + courseDTO.ToString());
          if (_context.Courses == null)
          {
              return Problem("Entity set 'SchoolContext.Courses'  is null.");
          }
            var course = new Course
            {
                CourseName = courseDTO.CourseName,
                StartDate = courseDTO.StartDate,
                EndDate = courseDTO.EndDate,
                LecturerId = courseDTO.LecturerId
            };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync(cancellationToken);

            if (courseDTO.Shifts != null && courseDTO.Shifts.Count >= 2)
            {
                foreach (var shift in courseDTO.Shifts)
                {
                    var courseShift = new Shift
                    {
                        CourseId = course.CourseId,
                        ShiftCode = shift.ShiftCode,
                        WeekDay = shift.WeekDay,
                        MaxQuantity = shift.MaxQuantity | 25
                    };
                    _context.Shifts.Add(courseShift);
                }

                await _context.SaveChangesAsync(cancellationToken); 
            }
            else
            {
                return BadRequest("Shifts must not null and have more than or equal to 2 shifts");
            }

            var courseOutput = await GetCourse(course.CourseId, cancellationToken); // Gọi tới phương thức GetCourse
            if (courseOutput.Result is NotFoundResult) // Kiểm tra nếu không tìm thấy
            {
                return NotFound(); // Trả về không tìm thấy
            }

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, courseOutput.Value);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute]int id, CancellationToken cancellationToken = default)
        {
            if (_context.Courses == null)
            {
                return NotFound();
            }
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        private bool CourseExists(int id)
        {
            return (_context.Courses?.Any(e => e.CourseId == id)).GetValueOrDefault();
        }
    }
}
