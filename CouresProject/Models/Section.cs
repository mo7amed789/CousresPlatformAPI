namespace CouresProject.Models
{
    public class Section
    {
        public string Title { get; internal set; }
        public int CourseId { get; internal set; }
        public int Order { get; internal set; }
        public object Lessons { get; internal set; }
        public int Id { get; internal set; }
    }
}
