using backend.Application.Services.Auth.Models;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace backend.Application.Services.Auth
{
    public class GoogleTokenValidator(IConfiguration configuration) : IGoogleTokenValidator
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<GoogleIdTokenPayload> ValidateAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")
                ?? _configuration.GetSection("GoogleAuth")["ClientId"]
                ?? throw new InvalidOperationException("GoogleAuth ClientId not found.");

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [clientId]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleIdTokenPayload
            {
                Subject = payload.Subject,
                Email = payload.Email,
                EmailVerified = payload.EmailVerified,
                Name = payload.Name,
                PictureUrl = payload.Picture
            };
        }
    }
}
