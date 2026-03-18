using AgendAI.Core.Domains.Staff;
using AgendAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.API.Endpoints;

public static class StaffEndpoints
{
    public static RouteGroupBuilder MapStaffEndpoints(this RouteGroupBuilder group)
    {
        // GET /api/v1/staff
        group.MapGet("/", async (AgendAIDbContext db) =>
        {
            var staff = await db.Staff
                .Where(s => s.Active)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Results.Ok(staff);
        }).RequireAuthorization("StaffOnly");

        // GET /api/v1/staff/{id}
        group.MapGet("/{id:guid}", async (Guid id, AgendAIDbContext db) =>
        {
            var staff = await db.Staff.FindAsync(id);

            return staff is null
                ? Results.NotFound()
                : Results.Ok(staff);
        }).RequireAuthorization("StaffOnly");

        // POST /api/v1/staff
        group.MapPost("/", async (CreateStaffRequest request, AgendAIDbContext db) =>
        {
            var branchExists = await db.Branches.AnyAsync(b => b.Id == request.BranchId);
            if (!branchExists)
                return Results.NotFound(new { message = "Branch não encontrada." });

            var staff = StaffMember.Create(
                request.BranchId,
                request.KeycloakUserId,
                request.Name,
                request.Phone,
                request.Specialty,
                request.CommissionPct);

            db.Staff.Add(staff);
            await db.SaveChangesAsync();

            return Results.Created($"/api/v1/staff/{staff.Id}", staff);
        }).RequireAuthorization("OwnerOnly");

        // PATCH /api/v1/staff/{id}/commission
        group.MapPatch("/{id:guid}/commission", async (Guid id, UpdateCommissionRequest request, AgendAIDbContext db) =>
        {
            var staff = await db.Staff.FindAsync(id);

            if (staff is null)
                return Results.NotFound();

            staff.UpdateCommission(request.CommissionPct);
            await db.SaveChangesAsync();

            return Results.Ok(staff);
        }).RequireAuthorization("OwnerOnly");

        // PATCH /api/v1/staff/{id}/deactivate
        group.MapPatch("/{id:guid}/deactivate", async (Guid id, AgendAIDbContext db) =>
        {
            var staff = await db.Staff.FindAsync(id);

            if (staff is null)
                return Results.NotFound();

            staff.Deactivate();
            await db.SaveChangesAsync();

            return Results.Ok(staff);
        }).RequireAuthorization("OwnerOnly");

        return group;
    }
}

// ─── Request Records ──────────────────────────────────────
public record CreateStaffRequest(
    Guid BranchId,
    string KeycloakUserId,
    string Name,
    string Phone,
    string Specialty,
    decimal CommissionPct);

public record UpdateCommissionRequest(decimal CommissionPct);
