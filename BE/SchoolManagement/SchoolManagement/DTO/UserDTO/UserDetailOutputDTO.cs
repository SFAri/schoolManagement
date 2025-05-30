using SchoolManagement.DTO.ScoreDTO;
using SchoolManagement.Models;

namespace SchoolManagement.DTO.UserDTO
{
    public class UserDetailOutputDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public RoleType RoleName { get; set; }
        public GenderType Gender { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public List<ScoreOuputDTO> Scores { get; set; }
    }
}
