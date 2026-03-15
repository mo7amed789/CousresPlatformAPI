using CouresProject.DTOs.Lessons;
using CouresProject.Services.Interfaces;
using CoursePlatformAPI.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatformAPI.Controllers
{
    [ApiController]
    [Route("api/lessons")]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _service;

        public LessonsController(ILessonService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateLessonDto dto)
        {
            var result = await _service.CreateLessonAsync(dto);

            return Ok(result);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateLessonDto dto)
        {
            var result = await _service.UpdateLessonAsync(id, dto);

            return Ok(result);
        }

        [Authorize(Roles = "Instructor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteLessonAsync(id);

            if (!result)
                return NotFound();

            return Ok();
        }
        public async Task<IActionResult> UploadVideo(
       [FromForm] VideoUploadDto dto,
       [FromServices] CloudinaryService cloudinary)
        {
            var file = dto.File;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (!file.ContentType.StartsWith("video/"))
                return BadRequest("Invalid file type");

            var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid video format");

            if (file.Length > 500 * 1024 * 1024)
                return BadRequest("File too large");

            var url = await cloudinary.UploadVideoAsync(file);

            return Ok(new { videoUrl = url });
        }

    }
}
