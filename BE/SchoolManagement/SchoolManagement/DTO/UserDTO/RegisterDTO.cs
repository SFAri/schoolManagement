using SchoolManagement.Models;

namespace SchoolManagement.DTO.UserDTO
{
    public class RegisterDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string Password { get; set; }
        public RoleType RoleType { get; set; } // Ví dụ: Admin, User
        public GenderType Gender { get; set; }
    }
}
