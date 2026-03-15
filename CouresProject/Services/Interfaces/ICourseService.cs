// ملف: CouresProject.Services.Interfaces/ICourseService.cs
using CouresProject.DTOs.Courses;
using CouresProject.Helpers;
using System.Security.Claims;

namespace CouresProject.Services.Interfaces
{
    public interface ICourseService
    {
        // دوال Pagination من المشروع الأول
        Task<PagedResult<CourseDto>> GetCoursesAsync(CourseQueryDto query);
        Task<CourseDetailsDto> GetCourseByIdAsync(int id);

        // دوال CRUD الأساسية
        Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, ClaimsPrincipal user);
        Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto dto, ClaimsPrincipal user);
        Task<bool> DeleteCourseAsync(int id, ClaimsPrincipal user);

        // دوال إضافية
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> GetSimpleCourseByIdAsync(int id);
        Task<IEnumerable<CourseDto>> SearchCoursesAsync(string searchTerm);
        Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(int instructorId);
    }
}