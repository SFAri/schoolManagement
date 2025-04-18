namespace SchoolManagement.Models
{
    public class Score
    {
        public int ScoreId { get; set; }
        public float ScoreNum { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        //Foreign
        public string? UserId { get; set; }
        public int? CourseId { get; set; }
        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
    }
}
