using CouresProject.Data;
using CouresProject.DTOs.Courses;
using CouresProject.Helpers;
using CouresProject.Models;
using CouresProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CouresProject.Repositories.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly CoursePlatformDbContext _context;

        public CourseRepository(CoursePlatformDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Course>> GetAllAsync(CourseQueryDto query)
        {
            var coursesQuery = _context.Courses
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                coursesQuery = coursesQuery.Where(c =>
                    c.Title.ToLower().Contains(search) ||
                    c.Description.ToLower().Contains(search));
            }

            if (query.MinPrice.HasValue)
                coursesQuery = coursesQuery.Where(c => c.Price >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                coursesQuery = coursesQuery.Where(c => c.Price <= query.MaxPrice.Value);

            var totalCount = await coursesQuery.CountAsync();

            var items = await coursesQuery
                .OrderByDescending(c => c.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Course>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Course course)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}
