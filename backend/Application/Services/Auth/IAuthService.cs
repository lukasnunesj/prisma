using backend.Application.DTO;
using backend.Domain.Entities;

namespace backend.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<(AuthResponseDTO Response, string RefreshToken, DateTime RefreshTokenExpires)> RegisterAsync(RegisterDTO registerDTO, string? ipAddress);
        Task<(AuthResponseDTO Response, string RefreshToken, DateTime RefreshTokenExpires)> LoginAsync(LoginDTO loginDTO, string? ipAddress);
        Task<(AuthResponseDTO Response, string RefreshToken, DateTime RefreshTokenExpires)> RefreshTokenAsync(string refreshToken, string? ipAddress);
        Task<(AuthResponseDTO Response, string RefreshToken, DateTime RefreshTokenExpires)> IssueTokensForUserAsync(User user, string? ipAddress);
        Task LogoutAsync(string refreshToken, string? ipAddress);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, string? ipAddress);
    }
}