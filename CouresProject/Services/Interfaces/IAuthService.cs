using CouresProject.DTOs.Auth;

namespace CouresProject.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        string GenerateJwtToken(string userId, string email, string role);
    }
}
