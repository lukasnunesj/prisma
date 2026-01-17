namespace backend.Application.Services.Auth.Models
{
    public class GoogleIdTokenPayload
    {
        public string Subject { get; init; } = null!;
        public string Email { get; init; } = null!;
        public bool EmailVerified { get; init; }
        public string? Name { get; init; }
        public string? PictureUrl { get; init; }
    }
}
