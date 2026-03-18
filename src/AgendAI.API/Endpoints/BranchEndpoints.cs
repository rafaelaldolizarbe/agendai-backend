using AgendAI.Core.Domains.Tenants;
using AgendAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.API.Endpoints;

public static class BranchEndpoints
{
    public static RouteGroupBuilder MapBranchEndpoints(this RouteGroupBuilder group)
    {
        // GET /api/v1/branches
        group.MapGet("/", async (AgendAIDbContext db) =>
        {
            var branches = await db.Branches
                .OrderBy(b => b.Name)
                .ToListAsync();

            return Results.Ok(branches);
        }).RequireAuthorization("StaffOnly");

        // GET /api/v1/branches/{id}
        group.MapGet("/{id:guid}", async (Guid id, AgendAIDbContext db) =>
        {
            var branch = await db.Branches.FindAsync(id);

            return branch is null
                ? Results.NotFound()
                : Results.Ok(branch);
        }).RequireAuthorization("StaffOnly");

        // POST /api/v1/branches
        group.MapPost("/", async (CreateBranchRequest request, AgendAIDbContext db) =>
        {
            var tenantExists = await db.Tenants.AnyAsync(t => t.Id == request.TenantId);
            if (!tenantExists)
                return Results.NotFound(new { message = "Tenant não encontrado." });

            var branch = Branch.Create(request.TenantId, request.Name, request.Address, request.Phone);
            db.Branches.Add(branch);
            await db.SaveChangesAsync();

            return Results.Created($"/api/v1/branches/{branch.Id}", branch);
        }).RequireAuthorization("OwnerOnly");

        // PATCH /api/v1/branches/{id}/deactivate
        group.MapPatch("/{id:guid}/deactivate", async (Guid id, AgendAIDbContext db) =>
        {
            var branch = await db.Branches.FindAsync(id);

            if (branch is null)
                return Results.NotFound();

            branch.Deactivate();
            await db.SaveChangesAsync();

            return Results.Ok(branch);
        }).RequireAuthorization("OwnerOnly");

        return group;
    }
}

// ─── Request Records ──────────────────────────────────────
public record CreateBranchRequest(Guid TenantId, string Name, string Address, string Phone);
