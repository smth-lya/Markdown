using MD.Application;
using MD.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MD.WebAPI;

[ApiController]
[Route("api/auth")]
public sealed class SignOutEndpoint : ControllerBase
{
    private readonly IUserService _userService;

    public SignOutEndpoint(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("signout")]
    [AllowAnonymous]
    public IActionResult SignOut(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(JwtTokenConstants.RefreshTokenType, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            return Ok();

        var jwtToken = new JwtToken(refreshToken);
        var result = _userService.SignOutAsync(jwtToken);

        Response.Cookies.Delete(JwtTokenConstants.RefreshTokenType);

        return Ok();
    }

    public record SignOutRequest(string RefreshToken);
}
