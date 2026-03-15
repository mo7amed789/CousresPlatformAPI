namespace CouresProject.DTOs.Courses
{
    public class CourseDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int InstructorId { get; set; }
        public List<SectionDto> Sections { get; set; } = new();
    }

    public class SectionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public List<LessonDto> Lessons { get; set; } = new();
    }

    public class LessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? VideoUrl { get; set; }
        public int Duration { get; set; }
        public int Order { get; set; }
    }
}
