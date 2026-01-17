using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using backend.Application.DTO;
using backend.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace backend.Tests;

public class PaymentMethodControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PaymentMethodControllerTests(CustomWebApplicationFactory factory)
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
        var response = await _client.GetAsync("/api/paymentmethod");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_ComTokenValidoESemRegistros_DeveRetornarNotFound()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/paymentmethod");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ComDadosValidos_DeveCriarFormaPagamento()
    {
        var token = await GetAuthTokenAsync();
        SetAuthHeader(token);

        var dto = new CreatePaymentMethodDTO
        {
            Name = "Cartão Débito",
            Type = PaymentMethodType.DEBITO
        };

        var response = await _client.PostAsJsonAsync("/api/paymentmethod", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<PaymentMethodResponseDTO>();
        created.Should().NotBeNull();
        created!.Id.Should().NotBeEmpty();
        created.Name.Should().Be(dto.Name);
        created.Type.Should().Be(dto.Type);
    }
}
