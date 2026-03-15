using CouresProject.Data;
using CouresProject.DTOs.Sections;
using CouresProject.Services.Interfaces;

using CouresProject.Models;

using Microsoft.EntityFrameworkCore;

namespace CouresProject.Services.Implementations
{
    public class SectionService : ISectionService
    {
        private readonly CoursePlatformDbContext _context;

        public SectionService(CoursePlatformDbContext context)
        {
            _context = context;
        }

        public async Task<object> CreateSectionAsync(CreateSectionDto dto)
        {
            var course = await _context.Courses.FindAsync(dto.CourseId);

            if (course == null)
                throw new Exception("Course not found");

            var section = new Section
            {
                Title = dto.Title,
                CourseId = dto.CourseId,
                Order = dto.Order
            };

            _context.Sections.Add(section);

            await _context.SaveChangesAsync();

            return section;
        }

        public async Task<object> UpdateSectionAsync(int id, CreateSectionDto dto)
        {
            var section = await _context.Sections.FindAsync(id);

            if (section == null)
                throw new Exception("Section not found");

            section.Title = dto.Title ?? section.Title;
            section.Order = dto.Order;

            await _context.SaveChangesAsync();

            return section;
        }

        public async Task<bool> DeleteSectionAsync(int id)
        {
            var section = await _context.Sections.FindAsync(id);

            if (section == null)
                return false;

            _context.Sections.Remove(section);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
