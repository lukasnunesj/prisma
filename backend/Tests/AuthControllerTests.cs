using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Application.DTO;
using backend.Application.Services.Auth;
using backend.Application.Services.Auth.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace backend.Tests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GoogleLogin_ComTokenValido_DeveRetornarToken()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IGoogleTokenValidator));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddScoped<IGoogleTokenValidator>(_ => new FakeGoogleTokenValidator());
            });
        }).CreateClient();

        var dto = new GoogleLoginDTO
        {
            IdToken = "fake-id-token"
        };

        var response = await client.PostAsJsonAsync("/api/auth/google", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.User.Email.Should().Be("teste-google@teste.com");
    }

    private class FakeGoogleTokenValidator : IGoogleTokenValidator
    {
        public Task<GoogleIdTokenPayload> ValidateAsync(string idToken, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new GoogleIdTokenPayload
            {
                Subject = "google-sub-123",
                Email = "teste-google@teste.com",
                EmailVerified = true,
                Name = "Teste Google"
            });
        }
    }

    [Fact]
    public async Task Register_ComDadosValidos_DeveRetornarToken()
    {
        // Arrange
        var registerDto = new RegisterDTO
        {
            Name = "Teste User",
            Email = $"teste{Guid.NewGuid()}@teste.com",
            Password = "Teste123!@#"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.User.Should().NotBeNull();
        authResponse.User.Email.Should().Be(registerDto.Email);
        authResponse.User.Name.Should().Be(registerDto.Name);
        authResponse.Expiration.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Register_ComEmailDuplicado_DeveRetornarBadRequest()
    {
        // Arrange
        var email = $"teste{Guid.NewGuid()}@teste.com";
        var registerDto1 = new RegisterDTO
        {
            Name = "Teste User 1",
            Email = email,
            Password = "Teste123!@#"
        };

        var registerDto2 = new RegisterDTO
        {
            Name = "Teste User 2",
            Email = email,
            Password = "Teste123!@#"
        };

        // Act
        await _client.PostAsJsonAsync("/api/auth/register", registerDto1);
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ComSenhaFraca_DeveRetornarBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDTO
        {
            Name = "Teste User",
            Email = $"teste{Guid.NewGuid()}@teste.com",
            Password = "123" // Senha muito fraca
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
    {
        // Arrange
        var email = $"teste{Guid.NewGuid()}@teste.com";
        var password = "Teste123!@#";

        var registerDto = new RegisterDTO
        {
            Name = "Teste User",
            Email = email,
            Password = password
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginDTO
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.User.Email.Should().Be(email);
    }

    [Fact]
    public async Task Login_ComEmailInvalido_DeveRetornarUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            Email = "emailinexistente@teste.com",
            Password = "Teste123!@#"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ComSenhaInvalida_DeveRetornarUnauthorized()
    {
        // Arrange
        var email = $"teste{Guid.NewGuid()}@teste.com";
        var registerDto = new RegisterDTO
        {
            Name = "Teste User",
            Email = email,
            Password = "Teste123!@#"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginDTO
        {
            Email = email,
            Password = "SenhaErrada123!@#"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_ComDadosInvalidos_DeveRetornarBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDTO
        {
            Name = "",
            Email = "email-invalido",
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

