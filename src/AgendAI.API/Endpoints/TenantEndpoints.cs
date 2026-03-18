using AgendAI.Core.Domains.Tenants;
using AgendAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.API.Endpoints;

public static class TenantEndpoints
{
    public static RouteGroupBuilder MapTenantEndpoints(this RouteGroupBuilder group)
    {
        // GET /api/v1/tenants
        group.MapGet("/", async (AgendAIDbContext db) =>
        {
            var tenants = await db.Tenants
                .OrderBy(t => t.Name)
                .ToListAsync();

            return Results.Ok(tenants);
        }).RequireAuthorization("OwnerOnly");

        // GET /api/v1/tenants/{id}
        group.MapGet("/{id:guid}", async (Guid id, AgendAIDbContext db) =>
        {
            var tenant = await db.Tenants.FindAsync(id);

            return tenant is null
                ? Results.NotFound()
                : Results.Ok(tenant);
        }).RequireAuthorization("OwnerOnly");

        // POST /api/v1/tenants
        group.MapPost("/", async (CreateTenantRequest request, AgendAIDbContext db) =>
        {
            var slugExists = await db.Tenants
                .AnyAsync(t => t.Slug == request.Slug.ToLower().Replace(" ", "-"));

            if (slugExists)
                return Results.Conflict(new { message = "Slug já está em uso." });

            var tenant = Tenant.Create(request.Name, request.Slug);
            db.Tenants.Add(tenant);
            await db.SaveChangesAsync();

            return Results.Created($"/api/v1/tenants/{tenant.Id}", tenant);
        }).RequireAuthorization("OwnerOnly");

        // PATCH /api/v1/tenants/{id}/deactivate
        group.MapPatch("/{id:guid}/deactivate", async (Guid id, AgendAIDbContext db) =>
        {
            var tenant = await db.Tenants.FindAsync(id);

            if (tenant is null)
                return Results.NotFound();

            tenant.Deactivate();
            await db.SaveChangesAsync();

            return Results.Ok(tenant);
        }).RequireAuthorization("OwnerOnly");

        return group;
    }
}

// ─── Request Records ──────────────────────────────────────
public record CreateTenantRequest(string Name, string Slug);
