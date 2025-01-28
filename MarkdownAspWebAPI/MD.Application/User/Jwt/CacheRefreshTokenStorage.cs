using MD.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace MD.Application;

public sealed class CacheRefreshTokenStorage : IJwtRefreshTokenStorage
{
    private const string REFRESH_TOKEN_PREFIX = "RefreshToken:";
    private const string VALID_STATUS = "valid";

    private const string INVALID_STATUS = "invalid";

    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheRefreshTokenStorage> _logger;

    public CacheRefreshTokenStorage(IMemoryCache cache, ILogger<CacheRefreshTokenStorage> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public bool IsValid(JwtToken refreshToken)
    {
        string key = REFRESH_TOKEN_PREFIX + refreshToken.Token;
        var value = _cache.Get<string>(key);

        return value is not null && value == VALID_STATUS;
    }

    public void Remove(JwtToken refreshToken)
    {
        string key = REFRESH_TOKEN_PREFIX + refreshToken.Token;
        _cache.Remove(key);
    }

    public void Store(JwtToken refreshToken, TimeSpan expirationTime)
    {
        string key = REFRESH_TOKEN_PREFIX + refreshToken.Token;

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        };

        _cache.Set(key, VALID_STATUS, cacheOptions);
    }
}