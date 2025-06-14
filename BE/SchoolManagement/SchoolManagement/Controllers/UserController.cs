using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.DTO.CourseDTO;
using SchoolManagement.DTO.ScoreDTO;
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
        [Authorize(Policy = "RequireAdminRole")]
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
                Gender = registerDto.Gender,
                DOB = registerDto.DOB
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password); // Khi tạo ban đầu sẽ lấy email làm mật khẩu cho tài khoản mới

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
                Console.WriteLine("user: " + user.RoleId);
                var userRoles = await _userManager.GetRolesAsync(user);
                Console.WriteLine("userRolessss: " + string.Join(", ", userRoles));
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                };

                //foreach (var userRole in userRoles)
                //{
                    //authClaims.Add();
                //}

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

        // Thay đổi mật khẩu
        //[AllowAnonymous]
        [HttpPost("reset-password/{id}")]
        public async Task<IActionResult> ResetPasswordAsync(string id ,ChangePasswordDTO pswDTO)
        {
            //string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //await _userManager.ResetPasswordAsync(user, token, password);
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            IdentityResult result = await _userManager.ChangePasswordAsync(user, pswDTO.OldPassword, pswDTO.NewPassword);
            
            if (result.Succeeded)
            {
                return Ok("Reset password of " + user.FirstName + " " + user.LastName + " successfully!");
            }
            List<IdentityError> errorList = result.Errors.ToList();
            var errors = string.Join(", ", errorList.Select(e => e.Description));
            return BadRequest(errors);

        }

        // Đăng xuất
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new { Message = "User logged out successfully." });
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDetailOutputDTO>> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Console.WriteLine("claim: ", ClaimTypes.NameIdentifier);
            Console.WriteLine("userId: ", userId);

            if (userId == null)
            {
                return Unauthorized(); // Nếu không tìm thấy userId trong claims
            }

            var user = await _userManager.Users
                .Include(u => u.Scores)
                .Where(u => u.Id == userId)
                .Select(u => new UserDetailOutputDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleName = u.RoleId,
                    Gender = u.Gender,
                    DOB = u.DOB,
                    Scores = u.Scores.Select(sc => new ScoreOuputDTO
                    {
                        AverageScore = sc.AverageScore,
                        CourseName = sc.Course.CourseName,
                        StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                        Process1 = sc.Process1,
                        Process2 = sc.Process2,
                        Midterm = sc.Midterm,
                        Final = sc.Final,
                        Grade = sc.Grade,
                        UserId = sc.UserId,
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // Lấy thông tin người dùng
        //[AllowAnonymous] // Just for test
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("all")]
        public async Task<ActionResult< IEnumerable<UserDetailOutputDTO>>> GetAllUser()
        {
            var users = await _userManager.Users
                .Include(u => u.Scores)
                .Include(u => u.Courses)
                .Where(u => u.RoleId != RoleType.Admin)
                .Select(u => new UserDetailOutputDTO {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleName = u.RoleId,
                    Gender = u.Gender,
                    DOB = u.DOB,
                    Scores = u.Scores.Select(sc => new ScoreOuputDTO
                    {
                        AverageScore = sc.AverageScore,
                        CourseName = sc.Course.CourseName,
                        StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                        Process1 = sc.Process1,
                        Process2 = sc.Process2,
                        Midterm = sc.Midterm,
                        Final = sc.Final,
                        UserId = sc.UserId,
                    }).ToList()
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
        public async Task<ActionResult<UserDetailOutputDTO>> GetUser(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Scores)
                .Where(u => u.Id == id)
                .Select(u => new UserDetailOutputDTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DOB = u.DOB,
                    Gender = u.Gender,
                    Email = u.Email,
                    Id = u.Id,
                    RoleName = u.RoleId,
                    Scores = u.Scores.Select(sc => new ScoreOuputDTO
                    {
                        AverageScore = sc.AverageScore,
                        CourseName = sc.Course.CourseName,
                        StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                        Process1 = sc.Process1,
                        Process2 = sc.Process2,
                        Midterm = sc.Midterm,
                        Final = sc.Final,
                        UserId = sc.UserId,
                    }).ToList()
                }).FirstOrDefaultAsync();
                //.ToListAsync();
            if (user == null)
            {
                return NotFound();
            }
            

            return Ok(user); // Trả về thông tin người dùng
        }

        // Cập nhật thông tin người dùng từng phần
        [AllowAnonymous]
        [HttpPatch("{id}")]
        public async Task<ActionResult<UserAllDTO>> PatchUser(string id, [FromBody] UserPatchInputDTO userDTO, CancellationToken cancellationToken = default)
        {
            if (userDTO == null)
            {
                return BadRequest();
            }
            //var user = await _context.Users.FindAsync(id);
            var user = await _userManager.FindByIdAsync(id);
            Console.WriteLine("=========== FINDDDDD: ", user);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin khóa học
            if (userDTO.FirstName != null)
            {
                user.FirstName = userDTO.FirstName;
            }
            if (userDTO.LastName != null)
            {
                user.LastName = userDTO.LastName;
            }
            if (userDTO.DOB != null)
            {
                user.DOB = (DateTime)userDTO.DOB;
            }
            if (userDTO.Email != null)
            {
                user.Email = userDTO.Email;
            }
            if (userDTO.Gender != null)
            {
                user.Gender = (GenderType)userDTO.Gender;
            }
            if (userDTO.RoleType != null)
            {
                await _userManager.RemoveFromRoleAsync(user, user.RoleId.ToString()); // Xóa role cũ
                user.RoleId = (RoleType)userDTO.RoleType; // Cập nhật RoleId
                await _userManager.AddToRoleAsync(user, userDTO.RoleType.ToString());
            }
            if (userDTO.Password != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, userDTO.Password);
            }

            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync(cancellationToken);

            var userOutput = await GetUser(user.Id);
            if (userOutput.Result is NotFoundResult)
            {
                return NotFound();
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, userOutput.Value);
        }

        // Xóa người dùng
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user); // Xóa người dùng
            return Ok($"User {user.FirstName} {user.LastName} deleted successfully.");
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
