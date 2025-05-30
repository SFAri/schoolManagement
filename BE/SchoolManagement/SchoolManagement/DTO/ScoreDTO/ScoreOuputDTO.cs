using SchoolManagement.Models;

namespace SchoolManagement.DTO.ScoreDTO
{
    public class ScoreOuputDTO
    {
        //public int ScoreId { get; set; }
        public string CourseName { get; set; } // Tên khóa học
        public int Year { get; set; } 
        public String UserId { get; set; } // Id sinh viên
        public string StudentName { get; set; } // Tên sinh viên
        public float Process1 { get; set; } // Điểm QT1
        public float Process2 { get; set; } // Điểm QT2
        public float Midterm { get; set; } // Điểm Giữa kì
        public float Final { get; set; } // Điểm Cuối kì
        public float AverageScore { get; set; } // Điểm trung bình
        public Grade Grade { get; set; } // Đánh giá học lực
        public DateTime CreatedAt { get; set; } // Ngày tạo
        public DateTime UpdatedAt { get; set; } // Ngày cập nhật
    }
}
