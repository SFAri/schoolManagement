using System;
namespace SchoolManagement.Models
{
    public class AcademicYear
    {
        public int AcademicYearId { get; set; }
        public string Year { get; set; } // Ví dụ: "2024-2025"
        public bool IsLocked { get; set; } = false; // Khi đã chốt sổ
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Course> Courses { get; set; }
    }

}

