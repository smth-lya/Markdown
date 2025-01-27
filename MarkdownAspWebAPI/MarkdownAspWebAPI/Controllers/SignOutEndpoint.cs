using MD.Application;
using MD.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MD.WebAPI;

[ApiController]
[Route("auth")]
public sealed class SignOutEndpoint : ControllerBase
{
    private readonly IUserService _userService;

    public SignOutEndpoint(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("signout")]
    [AllowAnonymous]
    public async Task<IActionResult> SignOut([FromBody] SignOutRequest request, CancellationToken ct)
    {
        var jwtToken = new JwtToken(request.RefreshToken);

        await _userService.SignOutAsync(jwtToken, ct);

        return Ok();
    }

    public record SignOutRequest(string RefreshToken);
}
