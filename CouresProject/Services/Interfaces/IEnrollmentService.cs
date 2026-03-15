using CouresProject.DTOs.Enrollment;
using System.Security.Claims;

namespace CouresProject.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<bool> EnrollAsync(EnrollDto dto, ClaimsPrincipal user);

        Task<IEnumerable<object>> GetMyCoursesAsync(ClaimsPrincipal user);

        Task<double> GetProgressAsync(int courseId, ClaimsPrincipal user);
    }
}
