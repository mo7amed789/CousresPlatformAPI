using Microsoft.AspNetCore.Mvc.ViewEngines;
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
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } // Student, Instructor, Admin
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties

        public ICollection<Course> CoursesCreated { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Review> Reviews { get; set; }




    }
}
