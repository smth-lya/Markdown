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
using System;
using Microsoft.EntityFrameworkCore.Design;

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
app.MapGet("user/add", async () =>
    "10 ASP Hello World!" + 
      
    await app.Services.CreateScope().ServiceProvider.GetRequiredService<IUserRepository>()
    .AddAsync(new User($"1@{Random.Shared.Next(0, 10000)}email", $"{Random.Shared.Next()}")));

//app.MapGet("/add", async () =>
//    "10 ASP Hello World!" + 
      
//    await app.Services.CreateScope().ServiceProvider.<IUserRepository>()
//    .AddAsync(new User($"1@{Random.Shared.Next(0, 10000)}email", $"{Random.Shared.Next()}")));



app.MapGet("/read", async () => 
    "10 ASP Hello World!" + 
    (await app.Services.CreateScope().ServiceProvider.GetRequiredService<IUserRepository>().GetUserByEmailAsync($"1@{Random.Shared.Next(0, 10000)}email"))
    .Value?.Email);

app.MapGet("/users", async (ApplicationDbContext dbcontext) =>
{
    var users = await dbcontext.Users
            .Select(u => $"Id: {u.Id} | Email: {u.Email} | PasswordHash: {u.PasswordHash}")
            .ToListAsync();

    string result = string.Join("\n", users);

    return string.IsNullOrEmpty(result) ? Results.NotFound("No users found") : Results.Text(result);
});

app.MapGet("/connection", (ApplicationDbContext dbContext) =>
{
    var connectionString = dbContext.Database.GetDbConnection().ConnectionString;
    return Results.Ok($"Connected to: {connectionString}");
});

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

app.Run();
return;