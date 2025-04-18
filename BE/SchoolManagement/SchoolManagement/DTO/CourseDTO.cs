using SchoolManagement.Models;

namespace SchoolManagement.DTO
{
    public class CourseDTO
    {
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? LecturerId { get; set; }
        public string? LecturerName { get; set; }
        public List<ShiftDTO>? Shifts { get; set; }
        public List<ScoreDTO>? Scores { get; set; }
    }
}
