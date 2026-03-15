using System.ComponentModel.DataAnnotations;

namespace CouresProject.DTOs.Sections
{
    public class CreateSectionDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }

        public int Order { get; set; }
    }
}
