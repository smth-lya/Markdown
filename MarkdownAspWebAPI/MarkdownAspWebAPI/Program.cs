using MD.Application;
using MD.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

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

//app.UseRouting();

app.UseAuthorization();

app.MapGet("/", async (context) =>
{
	try
	{
        var file = await File.ReadAllBytesAsync("wwwroot/index.html");

        context.Response.ContentType = "text/html;charset=utf-8";
        context.Response.ContentLength = file.Length;

        await context.Response.BodyWriter.WriteAsync(file);
    }
	catch (Exception ex)
	{

		throw;
	}

});

app.MapGet("/md-converter", async (context) =>
{
    try
    {
        var file = await File.ReadAllBytesAsync("wwwroot/converter.html");

        context.Response.ContentType = "text/html;charset=utf-8";
        context.Response.ContentLength = file.Length;

        await context.Response.BodyWriter.WriteAsync(file);
    }
    catch (Exception ex)
    {

        throw;
    }

});

app.MapGet("/file-storage", async (context) =>
{
    try
    {
        var file = await File.ReadAllBytesAsync("wwwroot/storage.html");

        context.Response.ContentType = "text/html;charset=utf-8";
        context.Response.ContentLength = file.Length;

        await context.Response.BodyWriter.WriteAsync(file);
    }
    catch (Exception ex)
    {
        throw;
    }

});

app.Run();