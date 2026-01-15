using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using backend.Application.DTO;
using FluentAssertions;
using Xunit;

namespace backend.Tests;

public class IncomeControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public IncomeControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var email = $"teste{Guid.NewGuid()}@teste.com";
        var password = "Teste123!@#";

        var registerDto = new RegisterDTO
        {
            Name = "Teste User",
            Email = email,
            Password = password
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDTO>();
        return authResponse!.Token;
    }

    private void SetAuthHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetAll_SemToken_DeveRetornarUnauthorized()
    {
        var response = await _client.GetAsync("/api/income/12/2099");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_ComTokenValidoESemEntradas_DeveRetornarNotFound()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/income/12/2099");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateEGetAll_ComDadosValidos_DeveRetornarEntradasNoPeriodo()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var createDto = new AddIncomeDTO
        {
            Name = "Salario",
            Amount = 1500.75m,
            Date = new DateOnly(2099, 12, 5)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/income/12/2099", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await _client.GetAsync("/api/income/12/2099");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var entradas = await getResponse.Content.ReadFromJsonAsync<List<IncomeResponseDTO>>();
        entradas.Should().NotBeNull();
        entradas!.Should().Contain(e => e.Name == createDto.Name);
    }
}
