using SchoolManagement.Models;

namespace SchoolManagement.DTO.ScoreDTO
{
    public class ScoreInputPutDTO
    {
        public int CourseId { get; set; }
        public String UserId { get; set; }
        public float? Process1 { get; set; }
        public float? Process2 { get; set; }
        public float? Midterm { get; set; }
        public float? Final { get; set; }
        public float? Average { get; set; }
        public Grade? Grade { get; set; }
    }
}
