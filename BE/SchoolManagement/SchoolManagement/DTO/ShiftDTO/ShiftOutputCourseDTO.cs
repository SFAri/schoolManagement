using SchoolManagement.Models;

namespace SchoolManagement.DTO.ShiftDTO
{
    public class ShiftOutputCourseDTO
    {
        public int ShiftId { get; set; }
        public ShiftOfDay ShiftOfDay { get; set; }
        public WeekDay WeekDay { get; set; }
        public int MaxQuantity { get; set; }
    }
}
