using SchoolManagement.Models;

namespace SchoolManagement.DTO.ShiftDTO
{
    public class ShiftInputDTO
    {
        public int CourseId { get; set; }
        public ShiftOfDay ShiftOfDay { get; set; }
        public WeekDay WeekDay { get; set; }
    }
}
