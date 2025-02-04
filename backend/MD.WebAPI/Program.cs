using System.Text.Json;
using System.Text.Json.Serialization;
using MD.Application;
using MD.Infrastructure;
using MD.WebAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Minio;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ������������ Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// ������������ Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 5 * 1024 * 1024; // 5 MB
});

// ������������ JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

// ������������ Minio
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));
builder.Services.AddSingleton<MinioClient>(provider =>
{
    var settings = provider.GetRequiredService<IOptions<MinioOptions>>().Value;
    return (MinioClient)new MinioClient()
        .WithEndpoint(settings.Endpoint)
        .WithCredentials(settings.AccessKey, settings.SecretKey)
        .WithSSL(false)
        .Build();
});

// ������������ �������� ������
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
});

// ������������ �������������� � �����������
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthConstants.ValidJwtTokenTypePolicy, policy =>
    {
        policy.RequireClaim(JwtTokenConstants.TokenTypeClaimName, JwtTokenConstants.AccessTokenType);
    });

// ����������� �������� ���������� � ��������������
builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration, builder.Environment);

// ����������� �������������� ��������
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureOptions<JwtBearerConfigureOptions>();

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
