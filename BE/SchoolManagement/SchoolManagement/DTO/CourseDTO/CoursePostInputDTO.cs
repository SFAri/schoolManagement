using SchoolManagement.DTO.ShiftDTO;

namespace SchoolManagement.DTO.CourseDTO
{
    public class CoursePostInputDTO
    {
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LecturerId { get; set; }
        public List<ShiftInputCourseDTO> Shifts { get; set; }
    }
}
