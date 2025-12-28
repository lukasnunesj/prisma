using backend.Application.DTO;
using backend.Application.Services.Auth;
using backend.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        private static CookieOptions BuildRefreshCookie(DateTime expires, bool isDevelopment)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDevelopment,
                SameSite = SameSiteMode.Strict,
                Expires = expires,
                Path = "/api/auth"
            };
        }

        private static CookieOptions BuildXsrfCookie(DateTime expires, bool isDevelopment)
        {
            return new CookieOptions
            {
                HttpOnly = false,
                Secure = !isDevelopment,
                SameSite = SameSiteMode.Strict,
                Expires = expires,
                Path = "/"
            };
        }

        private static string CreateXsrfToken()
        {
            var bytes = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private bool ValidateXsrf()
        {
            var xsrfCookie = Request.Cookies["XSRF-TOKEN"];
            var xsrfHeader = Request.Headers["X-XSRF-TOKEN"].ToString();

            if (string.IsNullOrWhiteSpace(xsrfCookie) || string.IsNullOrWhiteSpace(xsrfHeader))
                return false;

            return string.Equals(xsrfCookie, xsrfHeader, StringComparison.Ordinal);
        }

        [HttpGet("csrf")]
        public IActionResult Csrf()
        {
            var isDev = HttpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() ?? false;

            var config = HttpContext.RequestServices.GetService<IConfiguration>();
            var refreshDaysRaw = config?.GetSection("JwtSettings")["RefreshExpirationInDays"];
            var refreshDays = int.TryParse(refreshDaysRaw, out var parsedDays) ? parsedDays : 7;
            if (refreshDays <= 0)
                refreshDays = 7;

            var xsrf = CreateXsrfToken();
            Response.Cookies.Append("XSRF-TOKEN", xsrf, BuildXsrfCookie(DateTime.UtcNow.AddDays(refreshDays), isDev));
            return NoContent();
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var issued = await _authService.RegisterAsync(registerDTO, ip);

                var isDev = HttpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() ?? false;
                Response.Cookies.Append("refresh_token", issued.RefreshToken, BuildRefreshCookie(issued.RefreshTokenExpires, isDev));

                var xsrf = CreateXsrfToken();
                Response.Cookies.Append("XSRF-TOKEN", xsrf, BuildXsrfCookie(issued.RefreshTokenExpires, isDev));

                return Ok(issued.Response);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível registrar" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var issued = await _authService.LoginAsync(loginDTO, ip);

                var isDev = HttpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() ?? false;
                Response.Cookies.Append("refresh_token", issued.RefreshToken, BuildRefreshCookie(issued.RefreshTokenExpires, isDev));

                var xsrf = CreateXsrfToken();
                Response.Cookies.Append("XSRF-TOKEN", xsrf, BuildXsrfCookie(issued.RefreshTokenExpires, isDev));

                return Ok(issued.Response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Erro ao autenticar" });
            }

        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDTO>> Refresh()
        {
            try
            {
                if (!ValidateXsrf())
                    return Unauthorized(new { message = "Unauthorized" });

                var refreshCookie = Request.Cookies["refresh_token"];
                if (string.IsNullOrWhiteSpace(refreshCookie))
                    return Unauthorized(new { message = "Unauthorized" });

                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var issued = await _authService.RefreshTokenAsync(refreshCookie, ip);

                var isDev = HttpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() ?? false;
                Response.Cookies.Append("refresh_token", issued.RefreshToken, BuildRefreshCookie(issued.RefreshTokenExpires, isDev));

                var xsrf = CreateXsrfToken();
                Response.Cookies.Append("XSRF-TOKEN", xsrf, BuildXsrfCookie(issued.RefreshTokenExpires, isDev));

                return Ok(issued.Response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Erro" });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!ValidateXsrf())
                return Unauthorized(new { message = "Unauthorized" });

            var refreshCookie = Request.Cookies["refresh_token"];
            if (!string.IsNullOrWhiteSpace(refreshCookie))
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                await _authService.LogoutAsync(refreshCookie, ip);
            }

            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/auth" });
            Response.Cookies.Delete("XSRF-TOKEN", new CookieOptions { Path = "/" });

            return NoContent();
        }

        public class ChangePasswordDTO
        {
            public string CurrentPassword { get; set; } = null!;
            public string NewPassword { get; set; } = null!;
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ValidateXsrf())
                return Unauthorized(new { message = "Unauthorized" });

            var userId = User.GetUserId();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ok = await _authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword, ip);
            if (!ok)
                return BadRequest(new { message = "Não foi possível alterar a senha" });

            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/auth" });
            Response.Cookies.Delete("XSRF-TOKEN", new CookieOptions { Path = "/" });
            return NoContent();
        }
    }
}