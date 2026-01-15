using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using backend.Application.DTO;
using backend.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace backend.Tests;

public class TransactionControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TransactionControllerTests(CustomWebApplicationFactory factory)
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
        var response = await _client.GetAsync("/api/transaction/12/2099");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_ComTokenValidoESemTransacoes_DeveRetornarNotFound()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/transaction/12/2099");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateEGetAll_ComDadosValidos_DeveRetornarTransacaoNoPeriodo()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var categoryDto = new CreateCategoryDTO
        {
            Name = "Categoria teste",
            Color = "#FF00FF",
            ParentCategoryId = null
        };

        var categoryResponse = await _client.PostAsJsonAsync("/api/category", categoryDto);
        categoryResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdCategory = await categoryResponse.Content.ReadFromJsonAsync<CategoryResponseDto>();
        createdCategory.Should().NotBeNull();

        var createDto = new CreateTransactionDTO
        {
            Date = new DateOnly(2099, 12, 20),
            Amount = 123.45m,
            Type = TransactionType.INCOME,
            ExpenseType = null,
            Description = "Transacao teste",
            Mes = 12,
            Ano = 2099,
            CategoryId = createdCategory!.Id,
            IsPaid = false
        };

        var createResponse = await _client.PostAsJsonAsync("/api/transaction", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await _client.GetAsync("/api/transaction/12/2099");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var transactions = await getResponse.Content.ReadFromJsonAsync<List<TransactionResponseDTO>>();
        transactions.Should().NotBeNull();
        transactions!.Count.Should().BeGreaterThan(0);
    }
}
