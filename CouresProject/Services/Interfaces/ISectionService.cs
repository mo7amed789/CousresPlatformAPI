using CouresProject.DTOs.Sections;

namespace CouresProject.Services.Interfaces
{
    public interface ISectionService
    {
        Task<object> CreateSectionAsync(CreateSectionDto dto);

        Task<object> UpdateSectionAsync(int id, CreateSectionDto dto);

        Task<bool> DeleteSectionAsync(int id);
    }
}
