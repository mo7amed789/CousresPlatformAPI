namespace CouresProject.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string VideoUrl { get; set; }

        public int Duration { get; set; }

        public int Order { get; set; }


        // Foreign Key

        public int SectionId { get; set; }

        public Section Section { get; set; }
    }
}
