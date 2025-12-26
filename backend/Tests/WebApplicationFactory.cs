using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using backend.Infrastructure.Context;

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
                { "JwtSettings:ExpirationInMinutes", "30" }
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

