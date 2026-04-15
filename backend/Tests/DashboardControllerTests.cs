using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using backend.Application.DTO;
using FluentAssertions;
using Xunit;

namespace backend.Tests;

public class DashboardControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DashboardControllerTests(CustomWebApplicationFactory factory)
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
    public async Task Metrics_SemToken_DeveRetornarUnauthorized()
    {
        var response = await _client.GetAsync("/api/dashboard/metrics/12/2099");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Metrics_ComDados_DeveRetornarTotais()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var incomeDto = new CreateIncomeDTO
        {
            Name = "Salario",
            Amount = 2000m,
            Date = new DateOnly(2099, 12, 5)
        };

        var incomeResponse = await _client.PostAsJsonAsync("/api/income/12/2099", incomeDto);
        incomeResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var fixedExpenseDto = new CreateFixedExpenseDTO
        {
            Description = "Aluguel",
            Amount = 800m,
            Date = new DateOnly(2099, 12, 1),
            IsPaid = true
        };

        var fixedResponse = await _client.PostAsJsonAsync("/api/fixedexpense/12/2099", fixedExpenseDto);
        fixedResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var investmentDto = new CreateInvestmentDTO
        {
            Name = "Aportes",
            Amount = 200m,
            Date = new DateOnly(2099, 12, 2)
        };

        var investmentResponse = await _client.PostAsJsonAsync("/api/investment/12/2099", investmentDto);
        investmentResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var metricsResponse = await _client.GetAsync("/api/dashboard/metrics/12/2099");
        metricsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var metrics = await metricsResponse.Content.ReadFromJsonAsync<DashboardMetricsDTO>();
        metrics.Should().NotBeNull();
        metrics!.Totais.Entradas.Should().Be(2000m);
        metrics.Totais.Saidas.Should().Be(800m);
        metrics.Totais.Reservas.Should().Be(200m);
        metrics.Totais.Restante.Should().Be(1000m);
        metrics.Graficos.FluxoContas.Should().Contain(x => x.Nome == "Aluguel");
    }
}
