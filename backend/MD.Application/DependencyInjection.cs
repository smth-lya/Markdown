using Markdown.Core.Parsers;
using Markdown.Core.Processors;
using Markdown.Implementation;
using Markdown.Implementation.Parsers;
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

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IPermissionService, PermissionService>();

        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<IMarkdownProcessor, MarkdownProcessor>();
        services.AddScoped<IMarkdownParser, MarkdownToHtmlParser>();

        services.Configure<JwtOptions>(opts =>
        {
            configuration.GetSection("JwtOptions").Bind(opts);
        });

        return services;
    }
}
