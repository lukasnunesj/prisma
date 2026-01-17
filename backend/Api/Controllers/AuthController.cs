using backend.Api.Services;
using backend.Application.DTO;
using backend.Application.Services.Auth;
using backend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService, IGoogleTokenValidator googleTokenValidator, UserManager<User> userManager, ICurrentUser currentUser) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IGoogleTokenValidator _googleTokenValidator = googleTokenValidator;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ICurrentUser _currentUser = currentUser;

        private static CookieOptions BuildRefreshCookie(DateTime expires, bool isHttps)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = expires,
                Path = "/api/auth"
            };
        }

        [HttpPost("google")]
        public async Task<ActionResult<AuthResponseDTO>> GoogleLogin([FromBody] GoogleLoginDTO dto)
        {
            try
            {
                var payload = await _googleTokenValidator.ValidateAsync(dto.IdToken);
                if (!payload.EmailVerified)
                    return Unauthorized(new { message = "Email do Google não verificado" });

                var email = payload.Email.Trim().ToLowerInvariant();
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = email,
                        Email = email,
                        Name = payload.Name ?? email,
                        EmailConfirmed = true
                    };

                    var created = await _userManager.CreateAsync(user);
                    if (!created.Succeeded)
                        return BadRequest(new { message = "Não foi possível criar usuário" });
                }

                var logins = await _userManager.GetLoginsAsync(user);
                if (!logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == payload.Subject))
                {
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));
                }

                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var issued = await _authService.IssueTokensForUserAsync(user, ip);

                var isHttps = Request.IsHttps;
                Response.Cookies.Append("refresh_token", issued.RefreshToken, BuildRefreshCookie(issued.RefreshTokenExpires, isHttps));

                var xsrf = CreateXsrfToken();
                Response.Cookies.Append("XSRF-TOKEN", xsrf, BuildXsrfCookie(issued.RefreshTokenExpires, isHttps));

                return Ok(issued.Response);
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Google token inválido" });
            }
        }

        private static CookieOptions BuildXsrfCookie(DateTime expires, bool isHttps)
        {
            return new CookieOptions
            {
                HttpOnly = false,
                Secure = isHttps,
                SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
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
            var isHttps = Request.IsHttps;

            var config = HttpContext.RequestServices.GetService<IConfiguration>();
            var refreshDaysRaw = config?.GetSection("JwtSettings")["RefreshExpirationInDays"];
            var refreshDays = int.TryParse(refreshDaysRaw, out var parsedDays) ? parsedDays : 7;
            if (refreshDays <= 0)
                refreshDays = 7;

            var xsrf = CreateXsrfToken();
            Response.Cookies.Append("XSRF-TOKEN", xsrf, BuildXsrfCookie(DateTime.UtcNow.AddDays(refreshDays), isHttps));
            return NoContent();
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var issued = await _authService.RegisterAsync(registerDTO, ip);

                var isHttps = Request.IsHttps;
                Response.Cookies.Append("refresh_token", issued.RefreshToken, BuildRefreshCookie(issued.RefreshTokenExpires, isHttps));

                var xsrf = CreateXsrfToken();
                Response.Cookies.Append("XSRF-TOKEN", xsrf, BuildXsrfCookie(issued.RefreshTokenExpires, isHttps));

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

                var isHttps = Request.IsHttps;
                Response.Cookies.Append("refresh_token", issued.RefreshToken, BuildRefreshCookie(issued.RefreshTokenExpires, isHttps));

                var xsrf = CreateXsrfToken();
                Response.Cookies.Append("XSRF-TOKEN", xsrf, BuildXsrfCookie(issued.RefreshTokenExpires, isHttps));

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

                var isHttps = Request.IsHttps;
                Response.Cookies.Append("refresh_token", issued.RefreshToken, BuildRefreshCookie(issued.RefreshTokenExpires, isHttps));

                var xsrf = CreateXsrfToken();
                Response.Cookies.Append("XSRF-TOKEN", xsrf, BuildXsrfCookie(issued.RefreshTokenExpires, isHttps));

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

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ok = await _authService.ChangePasswordAsync(_currentUser.UserId, dto.CurrentPassword, dto.NewPassword, ip);
            if (!ok)
                return BadRequest(new { message = "Não foi possível alterar a senha" });

            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/auth" });
            Response.Cookies.Delete("XSRF-TOKEN", new CookieOptions { Path = "/" });
            return NoContent();
        }
    }
}