namespace backend.Application.DTO
{
    public class LoginDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegisterDTO
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
        public UserInfoDTO User { get; set; } = null!;
    }

    public class UserInfoDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;

    }
}