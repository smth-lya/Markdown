using Microsoft.AspNetCore.Http;
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

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid Id => GetClaimValue(ClaimTypes.NameIdentifier, Guid.Parse);
    public string Email => GetClaimValue(ClaimTypes.Email);
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