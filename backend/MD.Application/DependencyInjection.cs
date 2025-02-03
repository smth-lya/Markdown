using MD.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MD.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPasswordHasher, DefaultPasswordHasher>();

        services.AddScoped<IUserBusinessRulePredicates, DefaultUserBusinessRulePredicates>();

        services.AddScoped<IJwtEncoder, JwtEncoder>();
        services.AddScoped<IJwtRefreshTokenStorage, CacheRefreshTokenStorage>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IPermissionService, PermissionService>();

        services.AddScoped<ICurrentUser, CurrentUser>();

        services.Configure<JwtOptions>(opts =>
        {
            configuration.GetSection("JwtOptions").Bind(opts);
        });

        return services;
    }
}
