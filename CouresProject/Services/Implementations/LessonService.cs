using CouresProject.Data;
using CouresProject.DTOs.Lessons;
using CouresProject.Models;
using CouresProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CouresProject.Services.Implementations
{
    public class LessonService : ILessonService
    {
        private readonly CoursePlatformDbContext _context;

        public LessonService(CoursePlatformDbContext context)
        {
            _context = context;
        }

        public async Task<object> CreateLessonAsync(CreateLessonDto dto)
        {
            var section = await _context.Sections.FindAsync(dto.SectionId);

            if (section == null)
                throw new Exception("Section not found");

            var lesson = new Lesson
            {
                Title = dto.Title,
                SectionId = dto.SectionId,
                VideoUrl = dto.VideoUrl,
                Duration = dto.Duration,
                Order = dto.Order
            };

            _context.Lessons.Add(lesson);

            await _context.SaveChangesAsync();

            return lesson;
        }

        public async Task<object> UpdateLessonAsync(int id, CreateLessonDto dto)
        {
            var lesson = await _context.Lessons.FindAsync(id);

            if (lesson == null)
                throw new Exception("Lesson not found");

            lesson.Title = dto.Title ?? lesson.Title;
            lesson.VideoUrl = dto.VideoUrl ?? lesson.VideoUrl;
            lesson.Duration = dto.Duration;
            lesson.Order = dto.Order;

            await _context.SaveChangesAsync();

            return lesson;
        }

        public async Task<bool> DeleteLessonAsync(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);

            if (lesson == null)
                return false;

            _context.Lessons.Remove(lesson);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
