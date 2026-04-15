using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using backend.Application.DTO;
using FluentAssertions;
using Xunit;

namespace backend.Tests;

public class FixedExpenseControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FixedExpenseControllerTests(CustomWebApplicationFactory factory)
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
        var response = await _client.GetAsync("/api/fixedexpense/12/2099");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateEGetAll_ComDadosValidos_DeveRetornarContasNoPeriodo()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var createDto = new CreateFixedExpenseDTO
        {
            Description = "Internet",
            Amount = 199.90m,
            Date = new DateOnly(2099, 12, 3),
            IsPaid = false
        };

        var createResponse = await _client.PostAsJsonAsync("/api/fixedexpense/12/2099", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await _client.GetAsync("/api/fixedexpense/12/2099");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await getResponse.Content.ReadFromJsonAsync<List<FixedExpenseResponseDTO>>();
        list.Should().NotBeNull();
        list!.Should().Contain(x => x.Description == createDto.Description);
    }

    [Fact]
    public async Task Update_ComDadosValidos_DeveAtualizarConta()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var createDto = new CreateFixedExpenseDTO
        {
            Description = "Internet",
            Amount = 199.90m,
            Date = new DateOnly(2099, 12, 3),
            IsPaid = false
        };

        var createResponse = await _client.PostAsJsonAsync("/api/fixedexpense/12/2099", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<FixedExpenseResponseDTO>();
        created.Should().NotBeNull();

        var updateDto = new UpdateFixedExpenseDTO
        {
            Description = "Internet Fibra",
            Amount = 249.90m,
            Date = new DateOnly(2099, 12, 10),
            IsPaid = true
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/fixedexpense/{created!.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await updateResponse.Content.ReadFromJsonAsync<FixedExpenseResponseDTO>();
        updated.Should().NotBeNull();
        updated!.Description.Should().Be(updateDto.Description);
        updated.Amount.Should().Be(updateDto.Amount);
        updated.IsPaid.Should().BeTrue();
    }
}
