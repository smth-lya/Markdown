using MD.Application;
using MD.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
using Serilog;
using MD.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MD.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 5 * 1024 * 1024;
});

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration, builder.Environment);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthConstants.ValidJwtTokenTypePolicy, policy =>
    {
        policy.RequireClaim(JwtTokenConstants.TokenTypeClaimName, JwtTokenConstants.AccessTokenType);
    });

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.ConfigureOptions<JwtBearerConfigureOptions>();

var app = builder.Build();

app.UseStaticFiles();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "9 ASP Hello World!" + builder.Configuration.GetConnectionString("PostgresConnection"));
app.MapGet("/add", async () =>
    "10 ASP Hello World!" + 
    await app.Services.CreateScope().ServiceProvider.GetRequiredService<IUserRepository>()
    .AddAsync(new User("1@", "2323")));

app.MapGet("/read", async () => 
    "10 ASP Hello World!" + 
    (await app.Services.CreateScope().ServiceProvider.GetRequiredService<IUserRepository>().GetUserByEmailAsync("1@"))
    .Value?.Email);


app.MapGet("/home", async (context) =>
{
    var file = await File.ReadAllBytesAsync("wwwroot/index.html");

    context.Response.ContentType = "text/html;charset=utf-8";
    context.Response.ContentLength = file.Length;

    await context.Response.BodyWriter.WriteAsync(file);
});

app.MapGet("/md-converter", async (context) =>
{
    var file = await File.ReadAllBytesAsync("wwwroot/converter.html");

    context.Response.ContentType = "text/html;charset=utf-8";
    context.Response.ContentLength = file.Length;

    await context.Response.BodyWriter.WriteAsync(file);
});

app.MapGet("/file-storage", async (context) =>
{
    var file = await File.ReadAllBytesAsync("wwwroot/storage.html");

    context.Response.ContentType = "text/html;charset=utf-8";
    context.Response.ContentLength = file.Length;

    await context.Response.BodyWriter.WriteAsync(file);
});

if (app.Environment.IsDevelopment())
{
    MigrateDatabase(app);
}

app.Run();
return;
    
static void MigrateDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}