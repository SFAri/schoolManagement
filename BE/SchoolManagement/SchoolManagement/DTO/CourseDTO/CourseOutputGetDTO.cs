using SchoolManagement.DTO.ScoreDTO;
using SchoolManagement.DTO.ShiftDTO;
using SchoolManagement.DTO.UserDTO;

namespace SchoolManagement.DTO.CourseDTO
{
    public class CourseOutputGetDTO
    {
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public String? Year { get; set; }
        public LecturerOutputDTO? Lecturer { get; set; }
        public List<ShiftOutputCourseDTO>? Shifts { get; set; }
        public List<ScoreOuputDTO>? Scores { get; set; }

    }
}
