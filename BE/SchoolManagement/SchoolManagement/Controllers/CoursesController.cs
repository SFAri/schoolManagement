using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.DTO;
using SchoolManagement.DTO.Input;
using SchoolManagement.DTO.Output;
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
        public async Task<ActionResult<IEnumerable<CourseOutputGetDTO>>> GetCourses(int pageNumber = 1, int pageSize = 10)
        {
          if (_context.Courses == null)
          {
              return NotFound();
          }
            var courses = await _context.Courses
                  .OrderBy(c => c.CourseId)
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
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
                  .ToListAsync();

            return Ok(courses);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseOutputGetDTO>> GetCourse([FromRoute] int id)
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
                    ShiftOfDay = s.ShiftCode
                }).ToList(), // Chọn các thuộc tính mà bạn muốn từ Shift

                //Scores = c.Scores.Select(sc => new ScoreDTO
                //{
                //    StudentName = sc.User.FirstName + ' ' + sc.User.LastName, // Giả định rằng Score có thuộc tính StudentId
                //    ScoreNum = sc.ScoreNum, // Giả định rằng Score có thuộc tính ScoreNum
                //    CreatedAt = sc.CreatedAt,
                //    UpdatedAt = sc.LastUpdatedAt
                //}).ToList() // Chọn các thuộc tính mà bạn muốn từ Score
            })
            .FirstOrDefaultAsync(); // Lấy một khóa học đầu tiên (hoặc null)

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        // PATCH: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<ActionResult<CourseDTO>> PatchCourse([FromRoute]int id, [FromBody]CourseDTO courseDTO)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin khóa học
            if (courseDTO.CourseName != null)
            {
                course.CourseName = courseDTO.CourseName;
            }
            if (courseDTO.StartDate.HasValue)
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

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            courseDTO.CourseId = course.CourseId;
            return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CourseOutputGetDTO>> PostCourse([FromBody]CoursePostInputDTO courseDTO)
        {
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
            await _context.SaveChangesAsync();

            if (courseDTO.Shifts != null && courseDTO.Shifts.Count > 0)
            {
                foreach (var shift in courseDTO.Shifts)
                {
                    // Giả định rằng có một thực thể Shift có mối quan hệ với Course
                    var courseShift = new Shift
                    {
                        CourseId = course.CourseId,
                        ShiftCode = shift.ShiftCode,
                        WeekDay = shift.WeekDay,
                        MaxQuantity = shift.MaxQuantity | 25
                    };
                    _context.Shifts.Add(courseShift); // Thêm shift vào mối quan hệ khóa học
                }

                await _context.SaveChangesAsync(); // Lưu các shift đã thêm
            }

            //var courseOutput =
            //// await GetCourse(course.CourseId);
            //new CourseOutputGetDTO
            //{
            //    CourseId = course.CourseId,
            //    CourseName = course.CourseName,
            //    StartDate = course.StartDate,
            //    EndDate = course.EndDate,
            //    Lecturer = new LecturerOutputDTO
            //    {
            //        Email = course.Lecturer?.Email,
            //        LecturerId = course.Lecturer?.Id,
            //        LecturerName = course.Lecturer?.FirstName + ' ' + course.Lecturer?.LastName,
            //    },
            //    Shifts = course.Shifts?.Select(s => new ShiftOutputCourseDTO
            //    {
            //        ShiftId = s.ShiftId,
            //        WeekDay = s.WeekDay,
            //        ShiftOfDay = s.ShiftCode
            //    }).ToList()
            //};

            //return CreatedAtAction("GetCourse", new { id = course.CourseId }, courseOutput);
            var courseOutput = await GetCourse(course.CourseId); // Gọi tới phương thức GetCourse
            if (courseOutput.Result is NotFoundResult) // Kiểm tra nếu không tìm thấy
            {
                return NotFound(); // Trả về không tìm thấy
            }

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, courseOutput.Value);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
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
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(int id)
        {
            return (_context.Courses?.Any(e => e.CourseId == id)).GetValueOrDefault();
        }
    }
}
