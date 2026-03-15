using System.ComponentModel.DataAnnotations;

namespace CouresProject.DTOs.Lessons
{
    public class CreateLessonDto
    {
        [Required]
        public string Title { get; set; }

        public string VideoUrl { get; set; }

        public int Duration { get; set; }

        public int SectionId { get; set; }

        public int Order { get; set; }
    }
}
