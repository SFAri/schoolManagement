using SchoolManagement.Models;

namespace SchoolManagement.DTO.Input
{
    public class ShiftInputCourseDTO
    {
        public ShiftOfDay ShiftCode { get; set; }
        public WeekDay WeekDay { get; set; }
        public int MaxQuantity { get; set; }
    }
}
