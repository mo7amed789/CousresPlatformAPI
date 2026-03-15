namespace CouresProject.DTOs.Courses
{
    public class CourseQueryDto
    {
        private const int MaxPageSize = 50;

        public int Page { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
