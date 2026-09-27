using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductsService.Api.Auth;
using ProductsService.Api.Contracts;

namespace ProductsService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly DemoUserOptions _demoUser;

    public AuthController(ITokenService tokenService, IOptions<DemoUserOptions> demoUser)
    {
        _tokenService = tokenService;
        _demoUser = demoUser.Value;
    }

    /// <summary>
    /// Issues a JWT for the exercise's single demo account, so a reviewer can
    /// authenticate against the secured endpoints without standing up a full
    /// identity provider. See README for the credentials.
    /// </summary>
    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Token([FromBody] LoginRequest request)
    {
        if (request.Username != _demoUser.Username || request.Password != _demoUser.Password)
            return Unauthorized(new ProblemDetails { Title = "Invalid credentials", Status = StatusCodes.Status401Unauthorized });

        var result = _tokenService.GenerateToken(userId: "1", username: request.Username);

        return Ok(new LoginResponse(result.AccessToken, result.ExpiresAtUtc));
    }
}
