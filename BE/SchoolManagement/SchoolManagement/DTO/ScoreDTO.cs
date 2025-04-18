namespace SchoolManagement.DTO
{
    public class ScoreDTO
    {
        public int ScoreId { get; set; }
        public string CourseName { get; set; } // Tên khóa học
        public string StudentName { get; set; } // Tên sinh viên
        public float ScoreNum { get; set; } // Số điểm
        public DateTime CreatedAt { get; set; } // Ngày tạo
        public DateTime UpdatedAt { get; set; } // Ngày cập nhật
    }
}
