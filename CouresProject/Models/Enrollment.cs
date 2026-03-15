namespace CouresProject.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CourseId { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public double Progress { get; set; }


        // Navigation

        public User User { get; set; }

        public Course Course { get; set; }
    }
}
