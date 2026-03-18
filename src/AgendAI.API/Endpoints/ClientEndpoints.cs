using AgendAI.Core.Domains.Clients;
using AgendAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.API.Endpoints;

public static class ClientEndpoints
{
    public static RouteGroupBuilder MapClientEndpoints(this RouteGroupBuilder group)
    {
        // GET /api/v1/clients
        group.MapGet("/", async (AgendAIDbContext db) =>
        {
            var clients = await db.Clients
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Results.Ok(clients);
        }).RequireAuthorization("StaffOnly");

        // GET /api/v1/clients/{id}
        group.MapGet("/{id:guid}", async (Guid id, AgendAIDbContext db) =>
        {
            var client = await db.Clients.FindAsync(id);

            return client is null
                ? Results.NotFound()
                : Results.Ok(client);
        }).RequireAuthorization("StaffOnly");

        // GET /api/v1/clients/whatsapp/{whatsapp}
        group.MapGet("/whatsapp/{whatsapp}", async (string whatsapp, AgendAIDbContext db) =>
        {
            var client = await db.Clients
                .FirstOrDefaultAsync(c => c.WhatsApp == whatsapp);

            return client is null
                ? Results.NotFound()
                : Results.Ok(client);
        }).RequireAuthorization("StaffOnly");

        // POST /api/v1/clients
        group.MapPost("/", async (CreateClientRequest request, AgendAIDbContext db) =>
        {
            var client = Client.Create(
                request.KeycloakUserId,
                request.Name,
                request.Phone,
                request.WhatsApp);

            db.Clients.Add(client);
            await db.SaveChangesAsync();

            return Results.Created($"/api/v1/clients/{client.Id}", client);
        }).RequireAuthorization("AuthenticatedAny");

        return group;
    }
}

// ─── Request Records ──────────────────────────────────────
public record CreateClientRequest(
    string KeycloakUserId,
    string Name,
    string Phone,
    string WhatsApp);
