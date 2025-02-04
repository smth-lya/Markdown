using MD.Application;
using MD.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Minio;
using Npgsql;

namespace MD.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration, 
        IHostEnvironment environment)
    {   
        services.AddMemoryCache();


        if (!environment.IsEnvironment("Test"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }

                var dataSourceBuilder = new NpgsqlDataSourceBuilder("Host=db;Port=5432;Database=markdownapp;Username=postgres;Password=postgres");
                //var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("PostgresConnection"));
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();

                options.UseNpgsql(dataSource);
            });
        }

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserReadRepository>(sp => sp.GetRequiredService<IUserRepository>());

        services.AddScoped<IFileStorage, MinioFileStorage>();
        
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentReadRepository>(sp => sp.GetRequiredService<IDocumentRepository>());

        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IPermissionReadRepository>(sp => sp.GetRequiredService<IPermissionRepository>());

        return services;
    }
}
