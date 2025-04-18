using SchoolManagement.Models;

namespace SchoolManagement.DTO.Output
{
    public class ShiftOutputCourseDTO
    {
        public int ShiftId { get; set; }
        public ShiftOfDay ShiftOfDay { get; set; }
        public WeekDay WeekDay { get; set; }
    }
}
