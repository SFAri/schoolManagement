using SchoolManagement.Models;

namespace SchoolManagement.DTO.ShiftDTO
{
    public class ShiftInputCourseDTO
    {
        public int? ShiftId { get; set; }
        public ShiftOfDay ShiftCode { get; set; }
        public WeekDay WeekDay { get; set; }
        public int MaxQuantity { get; set; }
    }
}
