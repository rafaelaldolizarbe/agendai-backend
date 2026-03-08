using AgendAI.API.Endpoints;
using AgendAI.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ─── OpenAPI ─────────────────────────────────────────────
builder.Services.AddOpenApi();

// ─── Database ─────────────────────────────────────────────
builder.Services.AddDbContext<AgendAIDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// ─── Autenticação (Keycloak) ──────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false; // true em produção
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false, // Keycloak usa roles, não audience
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// ─── Middleware de autenticação ───────────────────────────
app.UseAuthentication();
app.UseAuthorization();

// ─── Health Check ─────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new { status = "healthy", app = "AgendAI API" }));

// ─── Rotas v1 ─────────────────────────────────────────────
var v1 = app.MapGroup("/api/v1");
v1.MapGet("/", () => Results.Ok(new { message = "AgendAI API v1" }));
v1.MapGroup("/scheduling").MapSchedulingEndpoints();

app.Run();
