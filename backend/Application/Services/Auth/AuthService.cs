using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using backend.Application.DTO;
using backend.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using backend.Infrastructure.Context;

namespace backend.Application.Services.Auth
{
    public class AuthService(UserManager<User> userManager, IConfiguration configuration, ApplicationDbContext context) : IAuthService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly backend.Infrastructure.Context.ApplicationDbContext _context = context;

        public async Task<(AuthResponseDTO Response, string RefreshToken, DateTime RefreshTokenExpires)> LoginAsync(LoginDTO loginDTO, string? ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                throw new UnauthorizedAccessException("Credenciais inválidas");
            }

            return await IssueTokensForUserAsync(user, ipAddress);
        }

        public async Task<(AuthResponseDTO Response, string RefreshToken, DateTime RefreshTokenExpires)> RegisterAsync(RegisterDTO registerDTO, string? ipAddress)
        {
            var user = new User
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                Name = registerDTO.Name
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return await IssueTokensForUserAsync(user, ipAddress);
        }

        public async Task<(AuthResponseDTO Response, string RefreshToken, DateTime RefreshTokenExpires)> IssueTokensForUserAsync(User user, string? ipAddress)
        {
            var claims = new List<Claim>{
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, user.Name)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new(ClaimTypes.Role, role));
            }

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not found.");
            var expirationMinutes = int.Parse(jwtSettings["ExpirationInMinutes"] ?? "30");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var refresh = CreateRefreshToken(user.Id, ipAddress);
            await _context.RefreshTokens.AddAsync(refresh.StoredToken);
            await _context.SaveChangesAsync();

            return (
                new AuthResponseDTO
            {
                Token = tokenString,
                Expiration = token.ValidTo,
                User = new UserInfoDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email ?? string.Empty
                }
            },
                refresh.RawToken,
                refresh.StoredToken.Expires
            );
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }

        private (string RawToken, RefreshToken StoredToken) CreateRefreshToken(Guid userId, string? ipAddress)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var refreshExpirationDays = int.Parse(jwtSettings["RefreshExpirationInDays"] ?? "7");

            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var raw = Convert.ToBase64String(randomBytes);
            var hash = HashToken(raw);

            return (raw, new RefreshToken
            {
                TokenHash = hash,
                Expires = DateTime.UtcNow.AddDays(refreshExpirationDays),
                UserId = userId,
                CreatedByIp = ipAddress
            });
        }

        private async Task RevokeAllRefreshTokensAsync(Guid userId, string? ipAddress)
        {
            var tokens = await _context.RefreshTokens.Where(r => r.UserId == userId && !r.IsRevoked).ToListAsync();
            if (tokens.Count == 0)
                return;

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<(AuthResponseDTO Response, string RefreshToken, DateTime RefreshTokenExpires)> RefreshTokenAsync(string refreshToken, string? ipAddress)
        {
            var tokenHash = HashToken(refreshToken);

            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
            if (storedToken == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            if (storedToken.IsRevoked)
            {
                // Se um refresh token já revogado foi apresentado, tratamos como possível replay.
                await RevokeAllRefreshTokensAsync(storedToken.UserId, ipAddress);
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            if (storedToken.Expires <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var newRefresh = CreateRefreshToken(user.Id, ipAddress);

            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress;
            storedToken.ReplacedByTokenHash = newRefresh.StoredToken.TokenHash;

            await _context.RefreshTokens.AddAsync(newRefresh.StoredToken);
            await _context.SaveChangesAsync();

            var issued = await IssueTokensForUserAsync(user, ipAddress);
            return (issued.Response, newRefresh.RawToken, newRefresh.StoredToken.Expires);
        }

        public async Task LogoutAsync(string refreshToken, string? ipAddress)
        {
            var tokenHash = HashToken(refreshToken);
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
            if (storedToken != null && !storedToken.IsRevoked)
            {
                storedToken.IsRevoked = true;
                storedToken.RevokedAt = DateTime.UtcNow;
                storedToken.RevokedByIp = ipAddress;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, string? ipAddress)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return false;

            await RevokeAllRefreshTokensAsync(userId, ipAddress);
            return true;
        }
    }
}