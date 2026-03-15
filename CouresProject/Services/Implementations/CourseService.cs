// ملف: CouresProject.Services.Implementations/CourseService.cs
using CouresProject.Data;
using CouresProject.DTOs.Courses;
using CouresProject.Helpers;
using CouresProject.Models;
using CouresProject.Repositories.Interfaces;
using CouresProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace CouresProject.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly CoursePlatformDbContext _context;
        private readonly ICacheService _cacheService;

        public CourseService(ICourseRepository courseRepository, CoursePlatformDbContext context, ICacheService cacheService)
        {
            _courseRepository = courseRepository;
            _context = context;
            _cacheService = cacheService;
        }

        // 1. GetCoursesAsync مع Pagination و Cache
        public async Task<PagedResult<CourseDto>> GetCoursesAsync(CourseQueryDto query)
        {
            // محاولة جلب البيانات من Cache
            var cacheKey = $"courses:{query.Page}:{query.PageSize}:{query.Search}:{query.MinPrice}:{query.MaxPrice}";
            var cachedData = await _cacheService.GetAsync(cacheKey);

            if (!string.IsNullOrWhiteSpace(cachedData))
            {
                return JsonSerializer.Deserialize<PagedResult<CourseDto>>(cachedData)!;
            }

            // إذا لم توجد في Cache, نجلبها من Repository
            var result = await _courseRepository.GetAllAsync(query);

            var mappedResult = new PagedResult<CourseDto>
            {
                Items = result.Items.Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price
                }),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };

            // حفظ النتيجة في Cache
            await _cacheService.SetAsync(cacheKey, JsonSerializer.Serialize(mappedResult), TimeSpan.FromMinutes(5));

            return mappedResult;
        }

        // 2. GetCourseByIdAsync مع التفاصيل الكاملة
        public async Task<CourseDetailsDto> GetCourseByIdAsync(int id)
        {
            // محاولة جلب الكورس من Cache
            var cacheKey = $"course_details:{id}";
            var cachedData = await _cacheService.GetAsync(cacheKey);

            if (!string.IsNullOrWhiteSpace(cachedData))
            {
                return JsonSerializer.Deserialize<CourseDetailsDto>(cachedData)!;
            }

            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new KeyNotFoundException("Course not found");

            var courseDetails = new CourseDetailsDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                InstructorId = course.InstructorId,
                Sections = System.Linq.Enumerable.OrderBy(course.Sections, (Section s) => s.Order)
                Sections = course.Sections
                    .OrderBy((Section s) => s.Order)
                    .Select(s => new SectionDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Order = s.Order,
                        Lessons = System.Linq.Enumerable.OrderBy(s.Lessons, (Lesson l) => l.Order)
                        Lessons = s.Lessons
                            .OrderBy((Lesson l) => l.Order)
                            .Select(l => new LessonDto
                            {
                                Id = l.Id,
                                Title = l.Title,
                                VideoUrl = l.VideoUrl,
                                Duration = l.Duration,
                                Order = l.Order
                            }).ToList()
                    }).ToList()
            };

            // حفظ في Cache لمدة 10 دقائق
            await _cacheService.SetAsync(cacheKey, JsonSerializer.Serialize(courseDetails), TimeSpan.FromMinutes(10));

            return courseDetails;
        }

        // 3. GetAllCoursesAsync (بدون Pagination)
        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            var cacheKey = "all_courses";
            var cachedData = await _cacheService.GetAsync(cacheKey);

            if (!string.IsNullOrWhiteSpace(cachedData))
            {
                return JsonSerializer.Deserialize<IEnumerable<CourseDto>>(cachedData)!;
            }

            var courses = await _context.Courses
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price
                })
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, JsonSerializer.Serialize(courses), TimeSpan.FromMinutes(5));

            return courses;
        }

        // 4. GetSimpleCourseByIdAsync (بدون تفاصيل)
        public async Task<CourseDto> GetSimpleCourseByIdAsync(int id)
        {
            var cacheKey = $"course_simple:{id}";
            var cachedData = await _cacheService.GetAsync(cacheKey);

            if (!string.IsNullOrWhiteSpace(cachedData))
            {
                return JsonSerializer.Deserialize<CourseDto>(cachedData)!;
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            var courseDto = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price
            };

            await _cacheService.SetAsync(cacheKey, JsonSerializer.Serialize(courseDto), TimeSpan.FromMinutes(10));

            return courseDto;
        }

        // 5. CreateCourseAsync
        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, ClaimsPrincipal user)
        {
            var instructorId = GetInstructorIdFromClaims(user);

            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                InstructorId = instructorId
            };

            await _courseRepository.AddAsync(course);

            // مسح Cache المتعلق بالكورسات
            await InvalidateCourseCacheAsync();

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price
            };
        }

        // 6. UpdateCourseAsync
        public async Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto dto, ClaimsPrincipal user)
        {
            var instructorId = GetInstructorIdFromClaims(user);

            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            if (course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You are not allowed to modify this course");

            if (!string.IsNullOrWhiteSpace(dto.Title))
                course.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                course.Description = dto.Description;

            if (dto.Price > 0)
                course.Price = dto.Price;

            await _courseRepository.UpdateAsync(course);

            // مسح Cache المتعلق بهذا الكورس وجميع الكورسات
            await InvalidateCourseCacheAsync(id);

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price
            };
        }

        // 7. DeleteCourseAsync
        public async Task<bool> DeleteCourseAsync(int id, ClaimsPrincipal user)
        {
            var instructorId = GetInstructorIdFromClaims(user);

            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return false;

            if (course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You are not allowed to delete this course");

            await _courseRepository.DeleteAsync(course);

            // مسح Cache المتعلق بهذا الكورس وجميع الكورسات
            await InvalidateCourseCacheAsync(id);

            return true;
        }

        // 8. SearchCoursesAsync
        public async Task<IEnumerable<CourseDto>> SearchCoursesAsync(string searchTerm)
        {
            // لا نستخدم Cache للبحث لأن النتائج متغيرة
            return await _context.Courses
                .Where(c => c.Title.Contains(searchTerm) ||
                           c.Description.Contains(searchTerm))
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price
                })
                .ToListAsync();
        }

        // 9. GetCoursesByInstructorAsync
        public async Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(int instructorId)
        {
            var cacheKey = $"instructor_courses:{instructorId}";
            var cachedData = await _cacheService.GetAsync(cacheKey);

            if (!string.IsNullOrWhiteSpace(cachedData))
            {
                return JsonSerializer.Deserialize<IEnumerable<CourseDto>>(cachedData)!;
            }

            var courses = await _context.Courses
                .Where(c => c.InstructorId == instructorId)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price
                })
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, JsonSerializer.Serialize(courses), TimeSpan.FromMinutes(5));

            return courses;
        }

        // دالة مساعدة خاصة
        private int GetInstructorIdFromClaims(ClaimsPrincipal user)
        {
            var instructorIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(instructorIdClaim))
                throw new UnauthorizedAccessException("Invalid token");

            return int.Parse(instructorIdClaim);
        }

        // دالة مساعدة لمسح Cache
        private async Task InvalidateCourseCacheAsync(int? courseId = null)
        {
            // مسح Cache الكورسات العامة
            await _cacheService.RemoveAsync("all_courses");

            // مسح Cache البحث (نستخدم Pattern Matching إذا كان مدعوماً)
            // إذا كان courseId موجود، نمسح Cache هذا الكورس
            if (courseId.HasValue)
            {
                await _cacheService.RemoveAsync($"course_details:{courseId}");
                await _cacheService.RemoveAsync($"course_simple:{courseId}");
            }

            // ملاحظة: مسح Cache الـ Pagination صعب لأن المفاتيح متعددة
            // يمكن إما تركها تنتهي طبيعياً أو استخدام Pattern Matching إذا كان Redis
        }
    }
}