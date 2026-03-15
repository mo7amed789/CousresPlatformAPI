using CouresProject.DTOs.Lessons;

namespace CouresProject.Services.Interfaces
{
    public interface ILessonService
    {
        Task<object> CreateLessonAsync(CreateLessonDto dto);

        Task<object> UpdateLessonAsync(int id, CreateLessonDto dto);

        Task<bool> DeleteLessonAsync(int id);
    }
}
