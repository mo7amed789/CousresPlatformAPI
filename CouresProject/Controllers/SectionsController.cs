using CouresProject.DTOs.Sections;
using CouresProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatformAPI.Controllers
{
    [ApiController]
    [Route("api/sections")]
    public class SectionsController : ControllerBase
    {
        private readonly ISectionService _service;

        public SectionsController(ISectionService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSectionDto dto)
        {
            var result = await _service.CreateSectionAsync(dto);

            return Ok(result);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateSectionDto dto)
        {
            var result = await _service.UpdateSectionAsync(id, dto);

            return Ok(result);
        }

        [Authorize(Roles = "Instructor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteSectionAsync(id);

            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
