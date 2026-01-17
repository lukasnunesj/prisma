using backend.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<backend.Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureAppConfiguration(config =>
    {
      config.AddInMemoryCollection(new Dictionary<string, string?>
        {
                { "ConnectionStrings:DefaultConnection", "Host=localhost;Port=5432;Database=testdb;Username=postgres;Password=postgres" },
                { "JwtSettings:SecretKey", "SuaChaveSecretaSuperSeguraComPeloMenos32Caracteres!" },
                { "JwtSettings:Issuer", "GerenteContas" },
                { "JwtSettings:Audience", "GerenteContasUsers" },
                { "JwtSettings:ExpirationInMinutes", "30" },
                { "JwtSettings:RefreshExpirationInDays", "7" },
                { "GoogleAuth:ClientId", "test-google-client-id" }
        });
    });

    builder.ConfigureServices(services =>
    {
      // Remove o DbContext real
      var descriptor = services.SingleOrDefault(
              d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

      if (descriptor != null)
      {
        services.Remove(descriptor);
      }

      // Adiciona DbContext em memória para testes
      // Usa um nome fixo para garantir que o banco seja compartilhado dentro da mesma instância
      services.AddDbContext<ApplicationDbContext>(options =>
          {
            options.UseInMemoryDatabase("TestDb");
          });
    });
  }
}

