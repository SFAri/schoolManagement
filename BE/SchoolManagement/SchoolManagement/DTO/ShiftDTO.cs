using SchoolManagement.Models;

namespace SchoolManagement.DTO
{
    public class ShiftDTO
    {
        public int? ShiftId { get; set; }
        //public string? ShiftName { get; set; }
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public ShiftOfDay? ShiftCode { get; set; }
        public WeekDay? Weekday { get; set; }
        public int? MaxQuantity { get; set; }
    }
}
