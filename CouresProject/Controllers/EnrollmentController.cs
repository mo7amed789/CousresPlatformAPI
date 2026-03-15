using CouresProject.DTOs.Enrollment;
using CouresProject.Models;
using CouresProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CouresProject.Controllers
{
    [ApiController]
    [Route("api/enrollment")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _service;

        public EnrollmentController(IEnrollmentService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Student")]
        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll(EnrollDto dto)
        {
            var result = await _service.EnrollAsync(dto, User);

            return Ok(new { message = "Enrolled successfully" });
        }

        [Authorize]
        [HttpGet("my-courses")]
        public async Task<IActionResult> MyCourses()
        {
            var courses = await _service.GetMyCoursesAsync(User);

            return Ok(courses);
        }

        [Authorize]
        [HttpGet("progress/{courseId}")]
        public async Task<IActionResult> Progress(int courseId)
        {
            var progress = await _service.GetProgressAsync(courseId, User);

            return Ok(new { progress });
        }
    }
}
