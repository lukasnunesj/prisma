using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using Xunit;

namespace backend.Tests;

public class SecurityTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public SecurityTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CategoryEndpoints_SemToken_DevemRetornarUnauthorized()
    {
        // Arrange & Act
        var getResponse = await _client.GetAsync("/api/category");
        var getByIdResponse = await _client.GetAsync($"/api/category/{Guid.NewGuid()}");
        var postResponse = await _client.PostAsJsonAsync("/api/category", new { Name = "Teste" });
        var putResponse = await _client.PutAsJsonAsync($"/api/category/{Guid.NewGuid()}", new { Name = "Teste" });
        var deleteResponse = await _client.DeleteAsync($"/api/category/{Guid.NewGuid()}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        putResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CategoryEndpoints_ComTokenInvalido_DevemRetornarUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token-invalido-12345");

        // Act
        var getResponse = await _client.GetAsync("/api/category");
        var getByIdResponse = await _client.GetAsync($"/api/category/{Guid.NewGuid()}");
        var postResponse = await _client.PostAsJsonAsync("/api/category", new { Name = "Teste" });
        var putResponse = await _client.PutAsJsonAsync($"/api/category/{Guid.NewGuid()}", new { Name = "Teste" });
        var deleteResponse = await _client.DeleteAsync($"/api/category/{Guid.NewGuid()}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        putResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CategoryEndpoints_ComTokenExpirado_DevemRetornarUnauthorized()
    {
        // Arrange
        // Token expirado simulado (formato JWT inválido/expirado)
        var tokenExpirado = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE1MTYyMzkwMjJ9.invalid";
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenExpirado);

        // Act
        var getResponse = await _client.GetAsync("/api/category");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthEndpoints_SemAutenticacao_DevemSerPublicos()
    {
        // Arrange & Act
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            Name = "Teste",
            Email = $"teste{Guid.NewGuid()}@teste.com",
            Password = "Teste123!@#"
        });

        // Assert
        registerResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CategoryEndpoints_ComTokenValido_DevemFuncionar()
    {
        // Arrange - Criar usuário e obter token
        var email = $"teste{Guid.NewGuid()}@teste.com";
        var password = "Teste123!@#";
        
        var registerDto = new backend.Application.DTO.RegisterDTO
        {
            Name = "Teste User",
            Email = email,
            Password = password
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<backend.Application.DTO.AuthResponseDTO>();
        var token = authResponse!.Token;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var getResponse = await _client.GetAsync("/api/category");

        // Assert
        // Pode retornar NotFound (sem categorias) ou OK (com categorias), mas não Unauthorized
        getResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CategoryEndpoints_ComTokenDeOutroUsuario_NaoDevemAcessarCategoriasDeOutroUsuario()
    {
        // Arrange - Criar dois usuários
        var email1 = $"teste1{Guid.NewGuid()}@teste.com";
        var email2 = $"teste2{Guid.NewGuid()}@teste.com";
        var password = "Teste123!@#";

        // Usuário 1
        var registerDto1 = new backend.Application.DTO.RegisterDTO
        {
            Name = "User 1",
            Email = email1,
            Password = password
        };
        var registerResponse1 = await _client.PostAsJsonAsync("/api/auth/register", registerDto1);
        var authResponse1 = await registerResponse1.Content.ReadFromJsonAsync<backend.Application.DTO.AuthResponseDTO>();
        var token1 = authResponse1!.Token;

        // Usuário 2
        var registerDto2 = new backend.Application.DTO.RegisterDTO
        {
            Name = "User 2",
            Email = email2,
            Password = password
        };
        var registerResponse2 = await _client.PostAsJsonAsync("/api/auth/register", registerDto2);
        var authResponse2 = await registerResponse2.Content.ReadFromJsonAsync<backend.Application.DTO.AuthResponseDTO>();
        var token2 = authResponse2!.Token;

        // Criar categoria com usuário 1
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);
        var createDto = new backend.Application.DTO.CreateCategoryDTO
        {
            Name = "Categoria User 1",
            ParentCategoryId = null
        };
        var createResponse = await _client.PostAsJsonAsync("/api/category", createDto);
        var categoria = await createResponse.Content.ReadFromJsonAsync<backend.Application.DTO.CategoryResponseDto>();

        // Tentar acessar com token do usuário 2
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

        // Act
        var getResponse = await _client.GetAsync($"/api/category/{categoria!.Id}");

        // Assert - Usuário 2 não deve conseguir acessar categoria do usuário 1
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CategoryEndpoints_ComDadosInvalidos_DevemRetornarBadRequest()
    {
        // Arrange
        var email = $"teste{Guid.NewGuid()}@teste.com";
        var registerDto = new backend.Application.DTO.RegisterDTO
        {
            Name = "Teste User",
            Email = email,
            Password = "Teste123!@#"
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<backend.Application.DTO.AuthResponseDTO>();
        var token = authResponse!.Token;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Tentar criar categoria com nome vazio
        var createDto = new backend.Application.DTO.CreateCategoryDTO
        {
            Name = "",
            ParentCategoryId = null
        };
        var response = await _client.PostAsJsonAsync("/api/category", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

