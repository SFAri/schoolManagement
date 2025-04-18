namespace SchoolManagement.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }
        //public string ShiftName { get; set; } = string.Empty;
        public ShiftOfDay ShiftCode { get; set; }
        public WeekDay WeekDay { get; set; } 
        public int MaxQuantity { get; set; }

        //Foreign
        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }
        public virtual IList<Enrollment>? Enrollments { get; set; }
    }

    public enum WeekDay
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }

    public enum ShiftOfDay
    {
        Morning,
        Afternoon
    }
}
