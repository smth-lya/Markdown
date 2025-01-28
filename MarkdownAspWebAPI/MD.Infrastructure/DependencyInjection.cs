using MD.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

                var dataSourceBuilder = new NpgsqlDataSourceBuilder("Host=db;Port=5432;Username=postgres;Password=2546;Database=markdownapp");
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();

                options.UseNpgsql(dataSource);
            });
        }

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserReadRepository>(sp => sp.GetRequiredService<IUserRepository>());

        return services;
    }
}