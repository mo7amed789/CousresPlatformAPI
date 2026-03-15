using CouresProject.Data;
using CouresProject.DTOs.Auth;
using CouresProject.Models;
using CouresProject.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace CouresProject.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly CoursePlatformDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(CoursePlatformDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (userExists)
                throw new Exception("Email already exists");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user.Id.ToString(), user.Email, user.Role);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
                throw new Exception("Invalid credentials");

            var validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!validPassword)
                throw new Exception("Invalid credentials");

            var token = GenerateJwtToken(user.Id.ToString(), user.Email, user.Role);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public string GenerateJwtToken(string userId, string email, string role)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(jwtSettings["DurationInMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
    
