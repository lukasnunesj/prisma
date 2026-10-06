using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace backend.Tests;

public class JwtKeyGuardTests
{
    [Fact]
    public void Startup_ForaDeDevelopment_ComChavePlaceholder_DeveFalhar()
    {
        using var factory = new CustomWebApplicationFactory()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Production"));

        var act = () => factory.CreateClient();

        act.Should().Throw<InvalidOperationException>().WithMessage("*placeholder*");
    }
}
