namespace ProductsService.Api.Auth;

/// <summary>
/// Stand-in credential store for this exercise. A real system would authenticate
/// against a user store (ASP.NET Core Identity, an IdP, etc.) instead of a single
/// configured account.
/// </summary>
public class DemoUserOptions
{
    public const string SectionName = "DemoUser";

    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
