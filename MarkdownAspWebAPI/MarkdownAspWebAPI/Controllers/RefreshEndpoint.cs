using MD.Application;
using MD.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MD.WebAPI;

[ApiController]
[Route("auth")]
public sealed class RefreshEndpoint : ControllerBase
{
    private readonly IJwtService _jwtService;

    public RefreshEndpoint(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, HttpContext context)
    {
        var jwtToken = new JwtToken(request.RefreshToken);

        var result = await _jwtService.RefreshAsync(jwtToken);

        if (!result.IsSuccess)
        {
            return BadRequest(string.Join("; ", result.Errors));
        }

        var refreshCookie = $"{JwtTokenConstants.RefreshTokenType}={result.Value.RefreshToken}; " +
                        $"HttpOnly; Secure; SameSite=Strict; Expires={DateTime.UtcNow.AddDays(7):R}";

        context.Response.Cookies.Append("Set-Cookie", refreshCookie);

        var response = new RefreshResponse(result.Value.AccessToken.Token, result.Value.RefreshToken.Token);
        return Ok(response);

    }

    public record RefreshRequest(string RefreshToken);

    public sealed record RefreshResponse(string AccessToken, string RefreshToken);
}
