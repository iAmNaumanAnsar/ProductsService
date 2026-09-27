using System.Net.Http.Json;

namespace ProductsService.IntegrationTests.Helpers;

public static class AuthHelper
{
    public static async Task<string> GetAccessTokenAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/token", new { username = "demo", password = "Passw0rd!" });
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return body!.AccessToken;
    }

    private record TokenResponse(string AccessToken, DateTime ExpiresAtUtc);
}
