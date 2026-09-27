using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Options;
using ProductsService.Api.Auth;
using Xunit;

namespace ProductsService.UnitTests.Auth;

public class TokenServiceTests
{
    private readonly TokenService _sut;
    private readonly JwtOptions _options = new()
    {
        Key = "unit-test-signing-key-needs-to-be-long-enough-256-bits",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpiryMinutes = 30
    };

    public TokenServiceTests()
    {
        _sut = new TokenService(Options.Create(_options));
    }

    [Fact]
    public void GenerateToken_ProducesTokenWithExpectedClaimsAndLifetime()
    {
        var result = _sut.GenerateToken(userId: "42", username: "demo");

        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.ExpiresAtUtc.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(5));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
        jwt.Issuer.Should().Be("TestIssuer");
        jwt.Audiences.Should().Contain("TestAudience");
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "42");
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == "demo");
    }
}
