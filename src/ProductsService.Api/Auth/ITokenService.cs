namespace ProductsService.Api.Auth;

public interface ITokenService
{
    TokenResult GenerateToken(string userId, string username);
}

public record TokenResult(string AccessToken, DateTime ExpiresAtUtc);
