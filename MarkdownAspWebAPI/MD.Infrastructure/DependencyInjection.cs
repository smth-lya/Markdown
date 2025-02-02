using MD.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("PostgresConnection"));
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
//public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
//{
//    public ApplicationDbContext CreateDbContext(string[] args)
//    {
//        // Создаем конфигурацию из переменных окружения
//        var configuration = new ConfigurationBuilder()
//            .AddEnvironmentVariables()
//            .Build();

//        var connectionString = configuration.GetConnectionString("PostgresConnection");

//        if (string.IsNullOrEmpty(connectionString))
//            throw new InvalidOperationException("Не найдена строка подключения в переменных окружения");

//        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
//        builder.UseNpgsql(
//            connectionString,
//            options => options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
//        );

//        return new ApplicationDbContext(builder.Options);
//    }
//}
