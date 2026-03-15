using System.ComponentModel.DataAnnotations;

namespace CouresProject.DTOs.Reviews
{
    public class CreateReviewDto
    {
        public int CourseId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }
    }
}
