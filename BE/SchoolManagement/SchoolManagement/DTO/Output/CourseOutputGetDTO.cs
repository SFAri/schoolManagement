namespace SchoolManagement.DTO.Output
{
    public class CourseOutputGetDTO
    {
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public LecturerOutputDTO? Lecturer { get; set; }
        public List<ShiftOutputCourseDTO>? Shifts { get; set; }
    }
}
