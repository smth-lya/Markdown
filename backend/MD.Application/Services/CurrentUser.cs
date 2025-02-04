using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MD.Application;

public interface ICurrentUser
{
    Guid Id { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
}

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CurrentUser> _logger;

    public CurrentUser(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUser> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public Guid Id => GetClaimValue(JwtRegisteredClaimNames.Sub, Guid.Parse);
    public string Email => GetClaimValue(JwtRegisteredClaimNames.Email) ?? throw new Exception("[CURRENT USER]: EMAIL NOT FOUND");
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    private T? GetClaimValue<T>(string claimType, Func<string, T> converter)
    {
        var value = GetClaimValue(claimType);
        return value is not null ? converter(value) : default;
    }

    private string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirst(claimType)?.Value;
    }
}