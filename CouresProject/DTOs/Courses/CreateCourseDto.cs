using System.ComponentModel.DataAnnotations;

namespace CouresProject.DTOs.Courses
{
    public class CreateCourseDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}
