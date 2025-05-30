namespace SchoolManagement.Models
{
    public class Score
    {
        //public int ScoreId { get; set; }
        public float Process1 { get; set; }
        public float Process2 { get; set; }
        public float Midterm { get; set; }
        public float Final { get; set; }
        public float AverageScore { get; set; }
        public Grade Grade { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        //Foreign
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public virtual User? User { get; set; }
        public virtual Course? Course { get; set; }
    }

    public enum Grade
    {
        Excellent,      // overall 9+
        VeryGood,       // overall 8+
        Good,           // overall 7+
        Average,        // overall 6+ 
        BelowAverge,    // overall < 6
        NotGraded       // firstInitial
    }
}
