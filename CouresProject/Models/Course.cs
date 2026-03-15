using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Collections.Specialized.BitVector32;

namespace CouresProject.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string Thumbnail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public User Instructor { get; set; }

        public ICollection<Section> Sections { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
