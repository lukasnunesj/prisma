using System.Security.Claims;
using backend.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace backend.Tests;

public class CurrentUserTests
{
    [Fact]
    public void UserId_QuandoNaoHaHttpContext_DeveLancarUnauthorized()
    {
        var accessor = new HttpContextAccessor { HttpContext = null };
        var currentUser = new CurrentUser(accessor);

        var act = () => currentUser.UserId;

        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void UserId_QuandoClaimInvalida_DeveLancarUnauthorized()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "not-a-guid")
        }, "Test"));

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new CurrentUser(accessor);

        var act = () => currentUser.UserId;

        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void UserId_QuandoClaimValida_DeveRetornarGuid()
    {
        var userId = Guid.NewGuid();
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "Test"));

        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new CurrentUser(accessor);

        currentUser.UserId.Should().Be(userId);
    }
}
