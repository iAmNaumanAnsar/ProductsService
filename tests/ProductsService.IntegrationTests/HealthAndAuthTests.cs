using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace ProductsService.IntegrationTests;

public class HealthAndAuthTests : IClassFixture<ProductsServiceWebApplicationFactory>
{
    private readonly ProductsServiceWebApplicationFactory _factory;

    public HealthAndAuthTests(ProductsServiceWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Health_IsAnonymouslyAccessibleAndReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Token_WithValidCredentials_ReturnsAccessToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/token", new { username = "demo", password = "Passw0rd!" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Token_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/token", new { username = "demo", password = "wrong-password" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
