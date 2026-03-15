using CouresProject.Data;
using CouresProject.DTOs.Enrollment;
using CouresProject.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CouresProject.Services.Interfaces;

namespace CouresProject.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly CoursePlatformDbContext _context;

        public EnrollmentService(CoursePlatformDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EnrollAsync(EnrollDto dto, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);

            var course = await _context.Courses.FindAsync(dto.CourseId);

            if (course == null)
                throw new Exception("Course not found");

            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == dto.CourseId);

            if (alreadyEnrolled)
                throw new Exception("Already enrolled");

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = dto.CourseId,
                Progress = 0
            };

            _context.Enrollments.Add(enrollment);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<object>> GetMyCoursesAsync(ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);

            return await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .Select(e => new
                {
                    CourseId = e.CourseId,
                    Title = e.Course.Title,
                    Progress = e.Progress
                })
                .ToListAsync();
        }

        public async Task<double> GetProgressAsync(int courseId, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId &&
                    e.CourseId == courseId
                );

            if (enrollment == null)
                throw new Exception("Not enrolled in this course");

            return enrollment.Progress;
        }
    }
}
