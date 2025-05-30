using SchoolManagement.Models;

namespace SchoolManagement.DTO.UserDTO
{
    public class UserPatchInputDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime? DOB { get; set; }
        public string? Password { get; set; }
        public RoleType? RoleType { get; set; } // Ví dụ: Admin, User
        public GenderType? Gender { get; set; }
    }
}
