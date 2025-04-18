using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

namespace SchoolManagement.Models
{
    public class SchoolContext : IdentityDbContext<User>
    {
        private readonly IConfiguration _configuration;
        private IDbConnection DbConnection { get; }
        public SchoolContext(DbContextOptions<SchoolContext> options, IConfiguration configuration)
        : base(options)
        {
            this._configuration = configuration;
            DbConnection = new SqlConnection(this._configuration.GetConnectionString("DevConnection"));
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbConnection.ToString());
            }
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
            }

        //public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Score> Scores { get; set; }

    }
}
