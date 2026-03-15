using System.ComponentModel.DataAnnotations;

namespace CouresProject.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(15,MinimumLength =3)]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // Student, Instructor, Admin
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties

        public ICollection<Course> CoursesCreated { get; set; } = new List<Course>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();




    }
}
