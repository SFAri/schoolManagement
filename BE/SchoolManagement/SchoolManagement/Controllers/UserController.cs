using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.DTO.UserDTO;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    //[Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager; // Sử dụng UserManager để quản lý người dùng
        private readonly SignInManager<User> _signInManager; // Để xác thực người dùng
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SchoolContext _context;

        public UserController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, SignInManager<User> signInManager, SchoolContext context, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        // Đăng ký người dùng mới
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName= registerDto.FirstName,
                LastName= registerDto.LastName,
                RoleId = registerDto.RoleType,
                DOB = DateTime.Today
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                var roleName = registerDto.RoleType.ToString();
                await _userManager.AddToRoleAsync(user, roleName);
                return Ok(new { Message = "Create new User successfully." });
            }

            return BadRequest(result.Errors); // Trả về lỗi nếu không thành công
        }

        // Đăng nhập người dùng
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    datetime = DateTime.Now
                });
            }

            return Unauthorized(new { Message = "Invalid login attempt." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new { Message = "User logged out successfully." });
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserAllDTO>> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Console.WriteLine("claim: ", ClaimTypes.NameIdentifier);
            Console.WriteLine("userId: ", userId);

            if (userId == null)
            {
                return Unauthorized(); // Nếu không tìm thấy userId trong claims
            }

            var user = await _userManager.FindByIdAsync(userId);


            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                RoleName = user.RoleId,
                user.DOB,
                user.Gender
            });
        }

        // Lấy thông tin người dùng
        [AllowAnonymous] // Just for test
        [HttpGet("all")]
        public async Task<ActionResult< IEnumerable<UserAllDTO>>> GetAllUser()
        {
            var users = await _userManager.Users
                .Select(u => new UserAllDTO { 
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleName = u.RoleId,
                    Gender = u.Gender,
                    DOB = u.DOB
                })
                .ToListAsync();

            if (users.Count == 0) return NotFound();

            return Ok(users); 
        }

        // Get all lecturer:
        [AllowAnonymous] // Just for test
        [HttpGet("all-lecturer")]
        public async Task<ActionResult<IEnumerable<UserAllDTO>>> GetAllLecturer()
        {
            var users = await _userManager.Users
                .Where(u => u.RoleId == RoleType.Lecturer)
                .Select(u => new UserAllDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleName = u.RoleId,
                })
                .ToListAsync();

            if (users.Count == 0) return NotFound();

            return Ok(users);
        }

        // Lấy thông tin người dùng
        [AllowAnonymous] // Just for test
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAllDTO>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var userOutput = new UserAllDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DOB = user.DOB,
                Gender = user.Gender,
                Email = user.Email,
                Id = user.Id,
                RoleName = user.RoleId
            };
            

            return userOutput; // Trả về thông tin người dùng
        }

        // Cập nhật thông tin người dùng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User userUpdate)
        {
            if (id != userUpdate.Id) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = userUpdate.Email;
            user.UserName = userUpdate.UserName;
            user.FirstName = userUpdate.FirstName;
            user.LastName = userUpdate.LastName;
            // Cập nhật các thuộc tính khác nếu cần

            await _userManager.UpdateAsync(user); // Cập nhật người dùng
            return NoContent();
        }

        // Xóa người dùng
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user); // Xóa người dùng
            return NoContent();
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
