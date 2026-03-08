using AgendAI.Core.Domains.Scheduling;
using AgendAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.API.Endpoints;

public static class SchedulingEndpoints
{
    public static RouteGroupBuilder MapSchedulingEndpoints(this RouteGroupBuilder group)
    {
        // GET /api/v1/scheduling — owner e hairdresser podem listar
        group.MapGet("/", async (AgendAIDbContext db) =>
        {
            var appointments = await db.Appointments
                .OrderBy(a => a.ScheduledAt)
                .ToListAsync();

            return Results.Ok(appointments);
        }).RequireAuthorization();

        // GET /api/v1/scheduling/{id}
        group.MapGet("/{id:guid}", async (Guid id, AgendAIDbContext db) =>
        {
            var appointment = await db.Appointments.FindAsync(id);

            return appointment is null
                ? Results.NotFound()
                : Results.Ok(appointment);
        }).RequireAuthorization();

        // POST /api/v1/scheduling — qualquer autenticado pode agendar
        group.MapPost("/", async (CreateAppointmentRequest request, AgendAIDbContext db) =>
        {
            var appointment = Appointment.Create(
                request.ClientId,
                request.StaffId,
                request.TenantId,
                request.Service,
                request.ScheduledAt,
                request.CreatedVia ?? "web");

            db.Appointments.Add(appointment);
            await db.SaveChangesAsync();

            return Results.Created($"/api/v1/scheduling/{appointment.Id}", appointment);
        }).RequireAuthorization();

        // PATCH /api/v1/scheduling/{id}/reschedule
        group.MapPatch("/{id:guid}/reschedule", async (Guid id, RescheduleRequest request, AgendAIDbContext db) =>
        {
            var appointment = await db.Appointments.FindAsync(id);

            if (appointment is null)
                return Results.NotFound();

            appointment.Reschedule(request.NewDate);
            await db.SaveChangesAsync();

            return Results.Ok(appointment);
        }).RequireAuthorization();

        // PATCH /api/v1/scheduling/{id}/cancel
        group.MapPatch("/{id:guid}/cancel", async (Guid id, AgendAIDbContext db) =>
        {
            var appointment = await db.Appointments.FindAsync(id);

            if (appointment is null)
                return Results.NotFound();

            appointment.Cancel();
            await db.SaveChangesAsync();

            return Results.Ok(appointment);
        }).RequireAuthorization();

        return group;
    }
}

// ─── Request Records ──────────────────────────────────────
public record CreateAppointmentRequest(
    Guid ClientId,
    Guid StaffId,
    Guid TenantId,
    string Service,
    DateTime ScheduledAt,
    string? CreatedVia);

public record RescheduleRequest(DateTime NewDate);
