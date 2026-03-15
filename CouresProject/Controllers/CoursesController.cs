using CouresProject.DTOs.Courses;
using CouresProject.Helpers;
using CouresProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CoursePlatformAPI.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _service;

        public CoursesController(ICourseService service)
        {
            _service = service;
        }
        [EnableRateLimiting("fixed")]
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] CourseQueryDto query)
        {
            var result = await _service.GetCoursesAsync(query);
            return Ok(ApiResponse<PagedResult<CourseDto>>.SuccessResponse(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var result = await _service.GetCourseByIdAsync(id);
            return Ok(ApiResponse<CourseDetailsDto>.SuccessResponse(result));
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            var result = await _service.CreateCourseAsync(dto, User);
            return Ok(ApiResponse<CourseDto>.SuccessResponse(result, "Course created successfully"));
        }

        [Authorize(Roles = "Instructor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
        {
            var result = await _service.UpdateCourseAsync(id, dto, User);
            return Ok(ApiResponse<CourseDto>.SuccessResponse(result, "Course updated successfully"));
        }

        [Authorize(Roles = "Instructor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _service.DeleteCourseAsync(id, User);

            if (!result)
                return NotFound(ApiResponse<string>.FailResponse("Course not found"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Course deleted successfully"));
        }
    }
}