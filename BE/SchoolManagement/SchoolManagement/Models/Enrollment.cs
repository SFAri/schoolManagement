namespace SchoolManagement.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public DateTime TimeJoined { get; set; }

        //Foreign
        public string UserId { get; set; }
        public int ShiftId { get; set; }
        public virtual User? User { get; set; }
        public virtual Shift? Shift { get; set; }
    }
}
