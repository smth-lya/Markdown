using MD.Application;
using MD.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MD.WebAPI;

[ApiController]
[Route("api/auth")]
public sealed class RefreshEndpoint : ControllerBase
{
    private readonly IJwtService _jwtService;

    public RefreshEndpoint(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue(JwtTokenConstants.RefreshTokenType, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var jwtToken = new JwtToken(refreshToken);
        var result = await _jwtService.RefreshAsync(jwtToken, cancellationToken);

        if (!result.IsSuccess)
        {
            return Unauthorized(string.Join("; ", result.Errors));
        }

        var tokenPair = result.Value;
        Response.Cookies.Append("refreshToken", tokenPair.RefreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        var response = new RefreshResponse(tokenPair.AccessToken.Token, tokenPair.RefreshToken.Token);
        return Ok(response);
    }

    public record RefreshRequest(string RefreshToken);

    public sealed record RefreshResponse(string AccessToken, string RefreshToken);
}
