using AgendAI.API.Endpoints;
using AgendAI.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text.Json;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AgendAIDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = "preferred_username",
            RoleClaimType = ClaimTypes.Role
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity identity) 
                    return Task.CompletedTask;

                // Keycloak coloca roles em realm_access.roles por padrão
                // Extraímos e mapeamos para ClaimTypes.Role do .NET
                var realmAccess = context.Principal
                    .FindFirstValue("realm_access");

                if (realmAccess is null) 
                    return Task.CompletedTask;

                using var doc = JsonDocument.Parse(realmAccess);
                if (!doc.RootElement.TryGetProperty("roles", out var roles))
                    return Task.CompletedTask;

                foreach (var role in roles.EnumerateArray())
                {
                    var roleName = role.GetString();
                    if (roleName is not null)
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OwnerOnly",        p => p.RequireRole("owner"));
    options.AddPolicy("StaffOnly",        p => p.RequireRole("owner", "hairdresser"));
    options.AddPolicy("AuthenticatedAny", p => p.RequireRole("owner", "hairdresser", "client"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", app = "AgendAI API" }));

var v1 = app.MapGroup("/api/v1");
v1.MapGet("/", () => Results.Ok(new { message = "AgendAI API v1" }));
v1.MapGroup("/scheduling").MapSchedulingEndpoints();

app.Run();
