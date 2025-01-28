using MD.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MD.WebAPI;

[ApiController]
[Route("api/auth")]
public sealed class ValidateEndpoint : ControllerBase
{
    private readonly IJwtEncoder _jwtEncoder;

    public ValidateEndpoint(IJwtEncoder jwtEncoder)
    {
        _jwtEncoder = jwtEncoder;
    }

    [HttpPost("/validate")]
    public IActionResult ValidateToken([FromHeader] ValidateTokenRequest request)
    {
        var jwtToken = new JwtToken(request.AccessToken);

        try
        {
            var claims = _jwtEncoder.ValidateToken(jwtToken);
        }
        catch (Exception)
        {
            return BadRequest("Invalid token.");
        }

        return Ok(new ValidateTokenResponse(true));
    }

    public sealed record ValidateTokenRequest(string AccessToken);

    public sealed record ValidateTokenResponse(bool IsValid);
}
