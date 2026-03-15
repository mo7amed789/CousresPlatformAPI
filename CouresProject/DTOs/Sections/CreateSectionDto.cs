using System.ComponentModel.DataAnnotations;

namespace CouresProject.DTOs.Sections
{
    public class CreateSectionDto
    {
        [Required]
        public string Title { get; internal set; }

        [Required]
        public int CourseId { get; set; }

        public int Order { get; set; }
    }
}
