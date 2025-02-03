using System.Text;
using MD.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MD.WebAPI;

public class JwtBearerConfigureOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _options;

    public JwtBearerConfigureOptions(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme) 
        {
            Configure(options);
        }
    }

    public void Configure(JwtBearerOptions options)
    {
        // ќтключает автоматическое преобразование (маппинг) типов утверждений (claims).
        // ѕо умолчанию, например, claim "sub" из JWT токена преобразуетс€ в
        // "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier".
        // ”становка этого параметра в false оставл€ет оригинальные имена claims из токена
        // (например, "sub" останетс€ "sub")
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateLifetime = true,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["s"];

                return Task.CompletedTask;
            }
        };
    }
}