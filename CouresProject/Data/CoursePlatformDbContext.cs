using CouresProject.Models;
using Microsoft.EntityFrameworkCore;
namespace CouresProject.Data
{
    public class CoursePlatformDbContext : DbContext
    {
        public CoursePlatformDbContext(DbContextOptions<CoursePlatformDbContext> options): base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Lesson> Lessons { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUser(modelBuilder);
            ConfigureCourse(modelBuilder);
            ConfigureEnrollment(modelBuilder);
            ConfigureReview(modelBuilder);
        }

        private void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        private void ConfigureCourse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.CoursesCreated)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureEnrollment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.UserId, e.CourseId })
                .IsUnique();
        }

        private void ConfigureReview(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CourseId);
        }
    }
}
