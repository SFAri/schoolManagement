using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.DTO;
using SchoolManagement.DTO.CourseDTO;
using SchoolManagement.DTO.ScoreDTO;
using SchoolManagement.DTO.ShiftDTO;
using SchoolManagement.DTO.UserDTO;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            //var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type.Contains("role"))?.Value;
            //var userId = User.Identity.GetUserId();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("User Id: " + userId);
            Console.WriteLine("User Role: " + userRole);

            // Switch case dựa trên userRole này
            var totalCourses = 0;
            List<CourseOutputGetDTO> courses;
            switch (userRole)
            {
                
                case "Admin":
                    totalCourses = await _context.Courses.CountAsync();
                    courses = await _context.Courses
                      .OrderBy(c => c.CourseId)
                      .Include(c => c.Lecturer)
                      .Include(c => c.Shifts)
                      .Include(c => c.AcademicYear)
                      .AsSplitQuery()
                      .Select(c => new CourseOutputGetDTO
                      {
                          CourseId = c.CourseId,
                          CourseName = c.CourseName,
                          StartDate = c.StartDate,
                          EndDate = c.EndDate,
                          Year = c.AcademicYear.Year,
                          Lecturer = new LecturerOutputDTO
                          {
                              Email = c.Lecturer.Email,
                              LecturerId = c.LecturerId,
                              LecturerName = c.Lecturer.FirstName + ' ' + c.Lecturer.LastName
                          },
                          Shifts = (List<ShiftOutputCourseDTO>)c.Shifts.Select(s => new ShiftOutputCourseDTO
                          {
                              ShiftId = s.ShiftId,
                              WeekDay = s.WeekDay,
                              ShiftOfDay = s.ShiftCode
                          }),
                          Scores = c.Scores.Select(sc => new ScoreOuputDTO
                          {
                              StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                              Process1 = sc.Process1,
                              Process2 = sc.Process2,
                              Midterm = sc.Midterm,
                              Final = sc.Final,
                              AverageScore = sc.AverageScore,
                              Grade = sc.Grade
                          })
                          .ToList() // Lấy danh sách điểm số
                      })
                      .ToListAsync(cancellationToken);
                    break;
                case "Lecturer":
                    courses = await _context.Courses
                      .OrderBy(c => c.CourseId)
                      .Include(c => c.Lecturer)
                      .Include(c => c.Shifts)
                      .Include(c => c.Scores)
                      .Include(c => c.AcademicYear)
                      .Where(c => c.Lecturer.Id == userId)
                      .AsSplitQuery()
                      .Select(c => new CourseOutputGetDTO
                      {
                          CourseId = c.CourseId,
                          CourseName = c.CourseName,
                          StartDate = c.StartDate,
                          EndDate = c.EndDate,
                          Year = c.AcademicYear.Year,
                          Lecturer = new LecturerOutputDTO
                          {
                              Email = c.Lecturer.Email,
                              LecturerId = c.LecturerId,
                              LecturerName = c.Lecturer.FirstName + ' ' + c.Lecturer.LastName
                          },
                          Shifts = (List<ShiftOutputCourseDTO>)c.Shifts
                            .Select(s => new ShiftOutputCourseDTO
                            {
                                ShiftId = s.ShiftId,
                                WeekDay = s.WeekDay,
                                ShiftOfDay = s.ShiftCode
                            }),
                          Scores = c.Scores.Select(sc => new ScoreOuputDTO
                          {
                              StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                              Process1 = sc.Process1,
                              Process2 = sc.Process2,
                              Midterm = sc.Midterm,
                              Final = sc.Final,
                              AverageScore = sc.AverageScore,
                              Grade = sc.Grade
                          })
                          .ToList() // Lấy danh sách điểm số
                      })
                      
                      .ToListAsync(cancellationToken);
                    totalCourses = courses.Count;
                    break;
                default: // Student
                    courses = await _context.Courses
                      .OrderBy(c => c.CourseId)
                      .Include(c => c.Lecturer)
                      .Include(c => c.Shifts)
                      .Where(c => c.Shifts.Any(s => _context.Enrollments.Any(e => e.ShiftId == s.ShiftId && e.UserId == userId)))
                      .Include(c => c.AcademicYear)
                      .AsSplitQuery()
                      .Select(c => new CourseOutputGetDTO
                      {
                          CourseId = c.CourseId,
                          CourseName = c.CourseName,
                          StartDate = c.StartDate,
                          EndDate = c.EndDate,
                          Year = c.AcademicYear.Year,
                          Lecturer = new LecturerOutputDTO
                          {
                              Email = c.Lecturer.Email,
                              LecturerId = c.LecturerId,
                              LecturerName = c.Lecturer.FirstName + ' ' + c.Lecturer.LastName
                          },
                          Shifts = (List<ShiftOutputCourseDTO>)c.Shifts
                              .Where(s => _context.Enrollments.Any(en => en.ShiftId == s.ShiftId && en.UserId == userId))
                              .Select(s => new ShiftOutputCourseDTO
                              {
                                  ShiftId = s.ShiftId,
                                  WeekDay = s.WeekDay,
                                  ShiftOfDay = s.ShiftCode
                              }),
                              Scores = c.Scores.Select(sc => new ScoreOuputDTO
                              {
                                  StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                                  Process1 = sc.Process1,
                                  Process2 = sc.Process2,
                                  Midterm = sc.Midterm,
                                  Final = sc.Final,
                                  AverageScore = sc.AverageScore,
                                  Grade = sc.Grade
                              })
                              .ToList() // Lấy danh sách điểm số
                      })
                      .ToListAsync(cancellationToken);
                    totalCourses = courses.Count;
                    break;
            }

            return Ok(new { data = courses, totalCount = totalCourses });
        }

        // GET: api/Courses/not-joined
        [HttpGet("not-joined")]
        public async Task<ActionResult<IEnumerable<CourseOutputGetDTO>>> GetCoursesStudent(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (_context.Courses == null)
            {
                return NotFound();
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("User Id: " + userId);

            // Switch case dựa trên userRole này
            var totalCourses = 0;
            var courses = await _context.Courses
                .OrderBy(c => c.CourseId)
                .Include(c => c.Lecturer)
                .Include(c => c.Shifts)
                .Include(c => c.AcademicYear)
                //.Include(c => c.Scores)     
                .AsSplitQuery()
                .ToListAsync(cancellationToken); // Load all courses first
            //var courses = await GetCourses();
            if (_context.Enrollments.Any()) // Check if there are any enrollments
            {
                Console.WriteLine("ENTER ANY ENROLLMENT: " + courses.ToString());
                courses = courses
                    .Where(c => {
                        Console.WriteLine("SHIFFTSSS: " + c.Shifts.Count);
                        return !c.Shifts.Any(s => _context.Enrollments.Any(e => e.ShiftId == s.ShiftId && e.UserId == userId)) && c.StartDate > DateTime.Now;
                    })
                    .ToList();
            }
            Console.WriteLine("LENGTH: " + courses.Count);

            var courseOutput = courses.Select(c => new CourseOutputGetDTO
             {
                 CourseId = c.CourseId,
                 CourseName = c.CourseName,
                 StartDate = c.StartDate,
                 EndDate = c.EndDate,
                 Year = c.AcademicYear.Year,
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
                 }).ToList()
                 //Scores = c.Scores.Select(sc => new ScoreDTO
                 //{
                 //    StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                 //    ScoreNum = sc.ScoreNum // Thay đổi nếu cần
                 //})
             })
            .ToList(); // Lấy danh sách điểm số
            return Ok(new { data = courseOutput, totalCount = totalCourses });
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
            .Include(c => c.AcademicYear)
            .Where(c => c.CourseId == id) // Tìm khóa học theo ID
            .AsSplitQuery()
            .Select(c => new CourseOutputGetDTO
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Year = c.AcademicYear.Year,
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
                    ShiftOfDay = s.ShiftCode,
                    MaxQuantity = s.MaxQuantity
                }).ToList(), // Chọn các thuộc tính mà bạn muốn từ Shift

                Scores = c.Scores.Select(sc => new ScoreOuputDTO
                {
                    UserId = sc.UserId,
                    StudentName = sc.User.FirstName + ' ' + sc.User.LastName, // Giả định rằng Score có thuộc tính StudentId
                    Process1 = sc.Process1,
                    Process2 = sc.Process2,
                    Midterm = sc.Midterm,
                    Final = sc.Final,
                    AverageScore = sc.AverageScore,
                    Grade = sc.Grade,
                    CreatedAt = sc.CreatedAt,
                    UpdatedAt = sc.LastUpdatedAt
                }).ToList() // Chọn các thuộc tính mà bạn muốn từ Score
            })
            .FirstOrDefaultAsync(cancellationToken); // Lấy một khóa học đầu tiên (hoặc null)

            if (course == null)
            {
                return NotFound();
            }

            // Check if the user is a student
            var isStudent = User.Claims.FirstOrDefault(c => c.Type.Contains("role"))?.Value == "Student";
            //var userId = User.Identity.GetUserId();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (isStudent)
            {
                // Filter shifts based on enrollment
                var enrolledShifts = await _context.Enrollments
                    .Include(e => e.Shift)
                    .Where(e => e.UserId == userId && e.Shift.CourseId == id)
                    .Select(e => e.ShiftId)
                    .ToListAsync(cancellationToken);

                // Filter the shifts in the course based on the enrolled shifts
                course.Shifts = course.Shifts.Where(s => enrolledShifts.Contains(s.ShiftId)).ToList();
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
            var course = await _context.Courses.Include(c => c.Shifts).FirstOrDefaultAsync(c => c.CourseId == id);
            Console.WriteLine("=========== FINDDDDD: ", course);
            if (course == null)
            {
                return NotFound();
            }

            // Kiểm tra trùng lặp thời gian
            //if (courseDTO.StartDate.HasValue || courseDTO.EndDate.HasValue)
            //{
            var startDate = courseDTO.StartDate ?? course.StartDate;
            var endDate = courseDTO.EndDate ?? course.EndDate;
            var lecturerId = courseDTO.LecturerId ?? course.LecturerId;
            // (1) shifts mới thì lấy từ DTO ; (2) shifts trong DTO null thì lấy shifts có sẵn trong db (tức là người dùng k thay đổi shifts có trong course hiện tại)
            var shifts = courseDTO.Shifts == null ?
                 course.Shifts.Select(s => new ShiftInputCourseDTO
                 {
                     WeekDay = s.WeekDay,
                     ShiftCode = s.ShiftCode,
                     MaxQuantity = s.MaxQuantity
                 })
                 : courseDTO.Shifts
                 .ToList();

            var overlappingCourses = await _context.Courses
                    .Where(c => c.CourseId != id &&
                                c.LecturerId == lecturerId &&
                                c.StartDate < endDate &&
                                c.EndDate > startDate)
                    .Select(c => c.CourseId)
                    .ToListAsync(cancellationToken);
                Console.WriteLine("== === LECTURERID: " + lecturerId);
                Console.WriteLine("== === OVERLAP COURSE: " + string.Join(", ", overlappingCourses.Select(c => c)));
                //Console.WriteLine("== === shift: " + string.Join(", ", shifts.Select(s => s.ShiftCode)));

            if (overlappingCourses.Any())
            {
                // Lấy các shifts của các khóa học trùng thời gian
                var overlappingShifts = await _context.Shifts
                    .Where(s => overlappingCourses.Contains(s.CourseId))
                    .Select(s => new { s.WeekDay, s.ShiftCode })
                    .ToListAsync(cancellationToken);

                var shiftKeySet = new HashSet<string>(overlappingShifts.Select(s => $"{(int)s.WeekDay}-{(int)s.ShiftCode}"));
                Console.WriteLine("== === shift key set: " + string.Join(", ", shiftKeySet.Select(s => s)));

                foreach (var shiftDTO in shifts)
                {
                    var key = $"{(int)shiftDTO.WeekDay}-{(int)shiftDTO.ShiftCode}";
                    Console.WriteLine("== KEY NÈ: " + key);
                    if (shiftKeySet.Contains(key))
                    {
                        return BadRequest($"Shift at: {shiftDTO.WeekDay} with time: {shiftDTO.ShiftCode} exists in another course of this lecturer.");
                    }
                }
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
            if (courseDTO.LecturerId != null)
            {
                course.LecturerId = courseDTO.LecturerId;
            }

            if (courseDTO.Shifts != null)
            {
                // Lấy danh sách shiftId từ courseDTO
                var shiftIdsInDTO = courseDTO.Shifts.Select(s => s.ShiftId).ToHashSet();

                // Xóa các shift không có shiftId trong courseDTO
                var shiftsToRemove = _context.Shifts
                    .Where(s => s.CourseId == course.CourseId && !shiftIdsInDTO.Contains(s.ShiftId))
                    .ToList();

                foreach (var item in shiftsToRemove)
                {
                    _context.Shifts.Remove(item);
                }

                // Thêm các shift mới
                foreach (var shiftDTO in courseDTO.Shifts)
                {
                    // Chỉ thêm các shift không có ShiftId trong cơ sở dữ liệu
                    if (shiftDTO.ShiftId == null || !_context.Shifts.Any(s => s.ShiftId == shiftDTO.ShiftId))
                    {
                        var newShift = new Shift
                        {
                            WeekDay = shiftDTO.WeekDay,
                            ShiftCode = shiftDTO.ShiftCode,
                            MaxQuantity = shiftDTO.MaxQuantity,
                            CourseId = course.CourseId
                        };
                        _context.Shifts.Add(newShift);
                    }
                }
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

            var courseOutput = await GetCourse(course.CourseId, cancellationToken);
            if (courseOutput.Result is NotFoundResult)
            {
                return NotFound();
            }

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, courseOutput.Value);
            //return Ok();
        }

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        public async Task<ActionResult<CourseOutputGetDTO>> PostCourse(CoursePostInputDTO courseDTO, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("CourseDTO: " + courseDTO.ToString());
              if (_context.Courses == null)
              {
                  return Problem("Entity set 'SchoolContext.Courses'  is null.");
              }
                var startDate = courseDTO.StartDate;
                var endDate = courseDTO.EndDate;
                var lecturerId = courseDTO.LecturerId;

            // Tạo danh sách khóa học trùng lịch (cùng giảng viên và thời gian chồng nhau)
            var overlappingCourses = await _context.Courses
                .Where(c =>
                    c.LecturerId == courseDTO.LecturerId &&
                    c.StartDate < courseDTO.EndDate &&
                    c.EndDate > courseDTO.StartDate
                )
                .Select(c => c.CourseId)
                .ToListAsync(cancellationToken);

            // Lấy toàn bộ các shifts của các khóa học trùng này
            var overlappingShifts = await _context.Shifts
                .Where(s => overlappingCourses.Contains(s.CourseId))
                .ToListAsync(cancellationToken);

            // Tạo danh sách key từ shifts trong DTO (dạng string "Weekday-ShiftCode")
            var newShiftKeys = courseDTO.Shifts?
                .Select(s => $"{(int)s.WeekDay}-{(int)s.ShiftCode}")
                .ToHashSet() ?? new HashSet<string>();

            // So sánh trong bộ nhớ để tìm trùng
            foreach (var shift in overlappingShifts)
            {
                var shiftKey = $"{(int)shift.WeekDay}-{(int)shift.ShiftCode}";
                if (newShiftKeys.Contains(shiftKey))
                {
                    return BadRequest($"Conflict: Shift {shift.ShiftCode} on {shift.WeekDay} already exists for this lecturer.");
                }
            }

            var latestYear = await _context.AcademicYears
                .OrderByDescending(y => y.CreatedAt)
                .FirstOrDefaultAsync(y => !y.IsLocked);

                    if (latestYear == null)
                    {
                        return BadRequest("Không có năm học nào đang mở. Vui lòng tạo năm học mới trước khi thêm khóa học.");
                    }

            var course = new Course
            {
                CourseName = courseDTO.CourseName,
                StartDate = courseDTO.StartDate,
                EndDate = courseDTO.EndDate,
                LecturerId = courseDTO.LecturerId,
                AcademicYearId = latestYear.AcademicYearId
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

        // Đăng ký khóa học mới cho student
        [Authorize(Policy = "RequireStudentRole")]
        [HttpPost("join")]
        public async Task<IActionResult> JoinCourse(CourseInputJoinDTO courseDTO, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("CourseDTO: " + courseDTO.ToString());
            if (courseDTO == null)
            {
                return BadRequest();
            }
            var course = await _context.Courses.Include(c => c.Shifts).FirstOrDefaultAsync(c => c.CourseId == courseDTO.CourseId);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course == null)
            {
                return NotFound();
            }

            Console.WriteLine("== === FIND COURSE: " + course.CourseName);

            // Kiểm tra xem có khóa nào trong danh sách các khóa student này đã đăng ký có shifts bị trùng khóa mới đăng ký này không?
            // 1. Lấy ds các khóa học student này đã join mà có thời gian trùng với thời gian khóa mới đang muốn đăng ký
            var shifts = course.Shifts
                .Where(s => courseDTO.ShiftsId.Contains(s.ShiftId))
                .Select(s => new ShiftInputCourseDTO
                 {
                     WeekDay = s.WeekDay,
                     ShiftCode = s.ShiftCode,
                     MaxQuantity = s.MaxQuantity
                 })
                 .ToList();

            var overlappingCourses = await _context.Courses
                    .Include(c => c.Shifts)
                    .Where( c =>
                        _context.Scores.Any(e => e.CourseId == c.CourseId && e.UserId == userId) &&
                        c.StartDate < course.EndDate &&
                        c.EndDate > course.StartDate)
                    .Select(c => c.CourseId)
                    .ToListAsync(cancellationToken);

            Console.WriteLine("== === OVERLAP COURSE: " + string.Join(", ", overlappingCourses.Select(c => c)));
            // 2. Lấy các shifts ở các courses đã lọc và so với shifts ở DTO
            if (overlappingCourses.Any())
            {
                var overlappingShifts = await _context.Shifts
                    .Where(s => overlappingCourses.Contains(s.CourseId))
                    .Select(s => new { s.WeekDay, s.ShiftCode })
                    .ToListAsync(cancellationToken);

                var shiftKeySet = new HashSet<string>(overlappingShifts.Select(s => $"{(int)s.WeekDay}-{(int)s.ShiftCode}"));
                Console.WriteLine("== === shift key set: " + string.Join(", ", shiftKeySet.Select(s => s)));

                foreach (var shiftDTO in shifts)
                {
                    var key = $"{(int)shiftDTO.WeekDay}-{(int)shiftDTO.ShiftCode}";
                    Console.WriteLine("== KEY NÈ: " + key);
                    if (shiftKeySet.Contains(key))
                    {
                        return BadRequest($"Shift: {shiftDTO.WeekDay} - {shiftDTO.ShiftCode} exists in another course this user have joined.");
                    }
                }
            }

            // Thực hiện thêm rowdata mới vào score table
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Create the score (only if one doesn't exist)
                    var existingScore = await _context.Scores.FirstOrDefaultAsync(s => s.CourseId == courseDTO.CourseId && s.UserId == userId);
                    if (existingScore == null)
                    {
                        var score = new Score
                        {
                            CourseId = courseDTO.CourseId,
                            UserId = userId,
                            Process1 = 0,
                            Process2 = 0,
                            Midterm = 0,
                            Final = 0,
                            AverageScore = 0,
                            Grade = Grade.NotGraded,
                            CreatedAt = DateTime.UtcNow,
                            LastUpdatedAt = DateTime.UtcNow
                        };
                        await _context.Scores.AddAsync(score);
                        await _context.SaveChangesAsync(cancellationToken); // Save changes to get the ScoreId
                    }

                    // 2. Add enrollments for each shift (only if they don't already exist)
                    foreach (var shiftId in courseDTO.ShiftsId.Distinct()) // Use Distinct() to prevent duplicate enrollments for the same shift
                    {
                        // Validate that the shift exists and belongs to the course
                        var shift = await _context.Shifts.FirstOrDefaultAsync(s => s.ShiftId == shiftId && s.CourseId == courseDTO.CourseId);
                        if (shift == null)
                        {
                            return BadRequest($"Shift with ID {shiftId} not found or does not belong to course {courseDTO.CourseId}.");
                        }

                        // Check if the user is already enrolled in this shift
                        var existingEnrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.ShiftId == shiftId && e.UserId == userId);
                        if (existingEnrollment == null)
                        {
                            var enrollment = new Enrollment
                            {
                                ShiftId = shiftId,
                                UserId = userId,
                                TimeJoined = DateTime.UtcNow
                            };
                            await _context.Enrollments.AddAsync(enrollment);
                        }
                    }

                    await _context.SaveChangesAsync(cancellationToken);

                    // Commit the transaction
                    transaction.Commit();

                    return Ok("Successfully joined course");
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    transaction.Rollback();
                    Console.Error.WriteLine(ex);
                    return StatusCode(500, "An error occurred while joining the course.");
                }
            }
        }

        // DELETE: unjoin a course:
        [Authorize(Policy = "RequireStudentRole")]
        [HttpDelete("unjoin/{id}")]
        public async Task<IActionResult> UnjoinCourse([FromRoute] int id, CancellationToken cancellationToken = default)
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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Xóa các score trước:
            var scoresToRemove = await _context.Scores.Where(sc => sc.CourseId == id && sc.UserId == userId).ToListAsync();
            if (scoresToRemove.Any())
            {
                _context.Scores.RemoveRange(scoresToRemove);
            }
            // Xóa các enrollment:
            var shiftsJoined = await _context.Shifts
                //.Include(c => c.Shifts)
                .Where(s => s.CourseId == id &&
                    _context.Scores.Any(sc => sc.UserId == userId && sc.CourseId == id))
                .Select(s => s.ShiftId)
                .ToListAsync();
            var enrollmentToRemove = await _context.Enrollments.Where(e => e.UserId == userId && shiftsJoined.Contains(e.ShiftId)).ToListAsync();

            if (enrollmentToRemove.Any())
            {
                _context.Enrollments.RemoveRange(enrollmentToRemove);
            }
            await _context.SaveChangesAsync(cancellationToken);

            return Ok("Unjoin course successfully!");
        }

        // DELETE: api/Courses/5
        [Authorize(Policy = "RequireAdminRole")]
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
