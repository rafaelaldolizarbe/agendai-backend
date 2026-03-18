using AgendAI.Core.Domains.Services;
using AgendAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.API.Endpoints;

public static class ServiceCatalogEndpoints
{
    public static RouteGroupBuilder MapServiceCatalogEndpoints(this RouteGroupBuilder group)
    {
        // GET /api/v1/services
        group.MapGet("/", async (AgendAIDbContext db) =>
        {
            var services = await db.ServiceCatalog
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Results.Ok(services);
        }).RequireAuthorization("AuthenticatedAny");

        // GET /api/v1/services/{id}
        group.MapGet("/{id:guid}", async (Guid id, AgendAIDbContext db) =>
        {
            var service = await db.ServiceCatalog.FindAsync(id);

            return service is null
                ? Results.NotFound()
                : Results.Ok(service);
        }).RequireAuthorization("AuthenticatedAny");

        // POST /api/v1/services
        group.MapPost("/", async (CreateServiceRequest request, AgendAIDbContext db) =>
        {
            var service = ServiceCatalog.Create(
                request.Name,
                request.Description,
                request.DurationMinutes);

            db.ServiceCatalog.Add(service);
            await db.SaveChangesAsync();

            return Results.Created($"/api/v1/services/{service.Id}", service);
        }).RequireAuthorization("OwnerOnly");

        return group;
    }
}

// ─── Request Records ──────────────────────────────────────
public record CreateServiceRequest(string Name, string Description, int DurationMinutes);
