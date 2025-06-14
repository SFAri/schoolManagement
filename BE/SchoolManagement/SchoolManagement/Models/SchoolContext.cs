using System.Data;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

namespace SchoolManagement.Models
{
    public class SchoolContext : IdentityDbContext<User>
    {
        //private readonly IConfiguration _configuration;
        //private IDbConnection DbConnection { get; }
        public SchoolContext(DbContextOptions<SchoolContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // one-to-one:
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Lecturer)
                .WithMany()
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            // one-to-many:
            modelBuilder.Entity<Shift>()
                .HasOne<Course>(s => s.Course)
                .WithMany(c => c.Shifts)
                .HasForeignKey(s => s.CourseId);

            // many-to-many:
            modelBuilder.Entity<Enrollment>().HasKey(en => new { en.UserId, en.ShiftId });
            modelBuilder.Entity<Score>().HasKey(sc => new { sc.UserId, sc.CourseId });

            modelBuilder.Entity<Enrollment>()
                .HasOne<User>(en => en.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(en => en.UserId);

            modelBuilder.Entity<Enrollment>()
                .HasOne<Shift>(sh => sh.Shift)
                .WithMany(sh => sh.Enrollments)
                .HasForeignKey(en => en.ShiftId);

            modelBuilder.Entity<Score>()
                .HasOne<User>(sc => sc.User)
                .WithMany(u => u.Scores)
                .HasForeignKey(sc => sc.UserId);

            modelBuilder.Entity<Score>()
                .HasOne<Course>(sc => sc.Course)
                .WithMany(c => c.Scores)
                .HasForeignKey(sc => sc.CourseId);

            // Property:
            modelBuilder.Entity<Shift>()
            .Property(s => s.WeekDay)
            .HasConversion<int>(); 

            modelBuilder.Entity<Shift>()
                .Property(s => s.ShiftCode)
                .HasConversion<int>();

            var adminRoleId = Guid.NewGuid().ToString();
            var lecturerRoleId = Guid.NewGuid().ToString();
            var studentRoleId = Guid.NewGuid().ToString();

            var adminUserId = Guid.NewGuid().ToString();
            var student1Id = Guid.NewGuid().ToString();
            var student2Id = Guid.NewGuid().ToString();
            var lecturerId = Guid.NewGuid().ToString();

            modelBuilder.Entity<AcademicYear>().HasData(
                new AcademicYear { AcademicYearId = 1, Year = "2023-2024", IsLocked = true, CreatedAt = new DateTime(2023, 6, 1) },
                new AcademicYear { AcademicYearId = 2, Year = "2024-2025", IsLocked = false, CreatedAt = DateTime.Now }
            );

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = lecturerRoleId, Name = "Lecturer", NormalizedName = "LECTURER" },
                new IdentityRole { Id = studentRoleId, Name = "Student", NormalizedName = "STUDENT" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminUserId,
                    UserName = "admin@gmail.com",
                    FirstName = "Katz",
                    LastName = "Ariz",
                    RoleId = RoleType.Admin, 
                    Gender = GenderType.Female,
                    Email = "admin@gmail.com",
                    NormalizedUserName = "ADMIN@GMAIL.COM",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "Admin123#"),
                },
                new User
                {
                    Id = lecturerId,
                    UserName = "lecturer1@gmail.com",
                    FirstName = "Liam",
                    LastName = "Nguyen",
                    Email = "lecturer1@gmail.com",
                    NormalizedUserName = "LECTURER1@GMAIL.COM",
                    NormalizedEmail = "LECTURER1@GMAIL.COM",
                    Gender = GenderType.Male,
                    RoleId = RoleType.Lecturer,
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "Lecturer123#")
                },
                new User
                {
                    Id = student1Id,
                    UserName = "student1@gmail.com",
                    FirstName = "Anna",
                    LastName = "Le",
                    Email = "student1@gmail.com",
                    NormalizedUserName = "STUDENT1@GMAIL.COM",
                    NormalizedEmail = "STUDENT1@GMAIL.COM",
                    Gender = GenderType.Female,
                    RoleId = RoleType.Student,
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "Student123#")
                },
                new User
                {
                    Id = student2Id,
                    UserName = "student2@gmail.com",
                    FirstName = "Minh",
                    LastName = "Tran",
                    Email = "student2@gmail.com",
                    NormalizedUserName = "STUDENT2@GMAIL.COM",
                    NormalizedEmail = "STUDENT2@GMAIL.COM",
                    Gender = GenderType.Male,
                    RoleId = RoleType.Student,
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "Student123#")
                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = adminUserId,
                    RoleId = adminRoleId
                },
                new IdentityUserRole<string>
                {
                    UserId = lecturerId,
                    RoleId = lecturerRoleId
                },
                new IdentityUserRole<string>
                {
                    UserId = student1Id,
                    RoleId = studentRoleId
                },
                new IdentityUserRole<string>
                {
                    UserId = student2Id,
                    RoleId = studentRoleId
                }
            );
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    CourseId = 1,
                    CourseName = "React",
                    StartDate = new DateTime(2023, 9, 1),
                    EndDate = new DateTime(2024, 1, 31),
                    LecturerId = lecturerId,
                    AcademicYearId = 1
                },
                new Course
                {
                    CourseId = 2,
                    CourseName = "Flutter",
                    StartDate = new DateTime(2023, 9, 1),
                    EndDate = new DateTime(2024, 1, 31),
                    LecturerId = lecturerId,
                    AcademicYearId = 1
                }
            );

            // === SEED SHIFT ===
            modelBuilder.Entity<Shift>().HasData(
                new Shift
                {
                    ShiftId = 1,
                    CourseId = 1,
                    ShiftCode = ShiftOfDay.Morning,
                    WeekDay = WeekDay.Monday,
                    MaxQuantity = 30
                },
                new Shift
                {
                    ShiftId = 2,
                    CourseId = 1,
                    ShiftCode = ShiftOfDay.Afternoon,
                    WeekDay = WeekDay.Thursday,
                    MaxQuantity = 30
                },
                new Shift
                {
                    ShiftId = 3,
                    CourseId = 2,
                    ShiftCode = ShiftOfDay.Morning,
                    WeekDay = WeekDay.Wednesday,
                    MaxQuantity = 30
                },
                new Shift
                {
                    ShiftId = 4,
                    CourseId = 2,
                    ShiftCode = ShiftOfDay.Afternoon,
                    WeekDay = WeekDay.Friday,
                    MaxQuantity = 30
                }
            );

            // === SEED ENROLLMENTS ===
            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment { ShiftId = 1, UserId = student1Id },
                new Enrollment { ShiftId = 2, UserId = student1Id },
                new Enrollment { ShiftId = 1, UserId = student2Id },
                new Enrollment { ShiftId = 2, UserId = student2Id },
                new Enrollment { ShiftId = 3, UserId = student1Id },
                new Enrollment { ShiftId = 4, UserId = student1Id },
                new Enrollment { ShiftId = 3, UserId = student2Id },
                new Enrollment { ShiftId = 4, UserId = student2Id }
            );

            // === SEED SCORES ===
            modelBuilder.Entity<Score>().HasData(
                new Score { CourseId = 1, UserId = student1Id, Process1 = 8.5f, Process2 = 9, Midterm = 10, Final= 8, AverageScore= 8.65f, Grade= Grade.VeryGood },
                new Score { CourseId = 1, UserId = student2Id, Process1 = 7f, Process2 = 6, Midterm = 10, Final = 7, AverageScore = 7.4f, Grade = Grade.Good },
                new Score { CourseId = 2, UserId = student1Id, Process1 = 8.5f, Process2 = 9, Midterm = 10, Final = 8, AverageScore = 8.65f, Grade = Grade.VeryGood },
                new Score { CourseId = 2, UserId = student2Id, Process1 = 6f, Process2 = 7, Midterm = 7, Final = 8, AverageScore = 7.4f, Grade = Grade.Good }
            );

        }

//public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<AcademicYear> AcademicYears { get; set; }
    }
}
