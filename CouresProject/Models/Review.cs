namespace CouresProject.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Foreign Keys

        public int UserId { get; set; }

        public int CourseId { get; set; }


        // Navigation

        public User User { get; set; } = null!;

        public Course Course { get; set; } = null!;
    }
}
