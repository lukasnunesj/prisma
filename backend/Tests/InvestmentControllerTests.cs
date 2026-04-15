using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using backend.Application.DTO;
using FluentAssertions;
using Xunit;

namespace backend.Tests;

public class InvestmentControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public InvestmentControllerTests(CustomWebApplicationFactory factory)
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
        var response = await _client.GetAsync("/api/investment/12/2099");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateEGetAll_ComDadosValidos_DeveRetornarInvestimentosNoPeriodo()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var createDto = new CreateInvestmentDTO
        {
            Name = "Aportes",
            Amount = 500.00m,
            Date = new DateOnly(2099, 12, 2)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/investment/12/2099", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await _client.GetAsync("/api/investment/12/2099");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await getResponse.Content.ReadFromJsonAsync<List<InvestmentResponseDTO>>();
        list.Should().NotBeNull();
        list!.Should().Contain(x => x.Name == createDto.Name);
    }

    [Fact]
    public async Task Update_ComDadosValidos_DeveAtualizarInvestimento()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var createDto = new CreateInvestmentDTO
        {
            Name = "Aportes",
            Amount = 500.00m,
            Date = new DateOnly(2099, 12, 2)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/investment/12/2099", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<InvestmentResponseDTO>();
        created.Should().NotBeNull();

        var updateDto = new UpdateInvestmentDTO
        {
            Name = "Aportes Atualizado",
            Amount = 750.00m,
            Date = new DateOnly(2099, 12, 10)
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/investment/{created!.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await updateResponse.Content.ReadFromJsonAsync<InvestmentResponseDTO>();
        updated.Should().NotBeNull();
        updated!.Id.Should().Be(created.Id);
        updated.Name.Should().Be(updateDto.Name);
        updated.Amount.Should().Be(updateDto.Amount);
    }
}
