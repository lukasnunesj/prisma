using backend.Application.Services.Auth.Models;

namespace backend.Application.Services.Auth
{
    public interface IGoogleTokenValidator
    {
        Task<GoogleIdTokenPayload> ValidateAsync(string idToken, CancellationToken cancellationToken = default);
    }
}
