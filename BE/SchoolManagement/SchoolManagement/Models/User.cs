using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace SchoolManagement.Models
{
    public class User : IdentityUser
    {
        //public string UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        //public string Email { get; set; } = null!;
        //public string Password { get; set; } = null!;
        public GenderType Gender { get; set; }
        public DateTime DOB { get; set; }

        public RoleType RoleId { get; set; } = RoleType.Student; // 0: admin, 1: lecturer, 2: student
        public virtual Course? Course { get; set; } // For lecturer
        public virtual IList<Enrollment>? Enrollments { get; set; }
        public virtual IList<Score>? Scores { get; set; }
    }

    public enum GenderType
    {
        Female,
        Male
    }

    public enum RoleType
    {
        Admin,
        Lecturer,
        Student
    }
}
