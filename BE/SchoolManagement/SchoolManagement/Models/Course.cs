﻿
using System.Text.Json.Serialization;

namespace SchoolManagement.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LecturerId { get; set; } = null!;
        public int AcademicYearId { get; set; }

        //foreign
        public virtual User? Lecturer { get; set; } // Lecturer
        public virtual AcademicYear? AcademicYear { get; set; }
        public virtual ICollection<Score>? Scores { get; set; }
        public virtual ICollection<Shift>? Shifts { get; set; }
    }
}
