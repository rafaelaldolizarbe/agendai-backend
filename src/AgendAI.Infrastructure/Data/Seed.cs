using AgendAI.Core.Domains.Clients;
using AgendAI.Core.Domains.Scheduling;
using AgendAI.Core.Domains.Services;
using AgendAI.Core.Domains.Staff;
using AgendAI.Core.Domains.Tenants;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infrastructure.Data;

public static class Seed
{
    public static async Task RunAsync(AgendAIDbContext db)
    {
        if (await db.Tenants.AnyAsync()) return; // já tem dados

        // ─── Tenant ───────────────────────────────────────
        var tenant = Tenant.Create("Salão da Maria", "salao-da-maria");
        db.Tenants.Add(tenant);

        // ─── Branch ───────────────────────────────────────
        var branch = Branch.Create(
            tenant.Id,
            "Unidade Centro",
            "Rua das Flores, 123 — Centro",
            "(11) 99999-0001");
        db.Branches.Add(branch);

        // ─── Service Catalog ──────────────────────────────
        var corte = ServiceCatalog.Create("Corte feminino", "Corte e finalização", 60);
        var corteM = ServiceCatalog.Create("Corte masculino", "Corte simples", 30);
        var coloracao = ServiceCatalog.Create("Coloração", "Coloração completa", 120);
        var manicure = ServiceCatalog.Create("Manicure", "Unhas das mãos", 45);
        var pedicure = ServiceCatalog.Create("Pedicure", "Unhas dos pés", 45);
        db.ServiceCatalog.AddRange(corte, corteM, coloracao, manicure, pedicure);

        // ─── Staff ────────────────────────────────────────
        var juliana = StaffMember.Create(
            branch.Id,
            "keycloak-hairdresser-id",
            "Juliana Silva",
            "(11) 99999-0002",
            "Cabeleireira",
            40);

        var fernanda = StaffMember.Create(
            branch.Id,
            "keycloak-manicure-id",
            "Fernanda Santos",
            "(11) 99999-0003",
            "Manicure",
            35);

        db.Staff.AddRange(juliana, fernanda);

        // ─── Staff Services ───────────────────────────────
        db.StaffServices.AddRange(
            StaffService.Create(juliana.Id, corte.Id, 80),
            StaffService.Create(juliana.Id, corteM.Id, 40),
            StaffService.Create(juliana.Id, coloracao.Id, 200),
            StaffService.Create(fernanda.Id, manicure.Id, 35),
            StaffService.Create(fernanda.Id, pedicure.Id, 35)
        );

        // ─── Clients ──────────────────────────────────────
        var ana = Client.Create("keycloak-client-id", "Ana Souza", "(11) 99999-0004", "5511999990004");
        var maria = Client.Create("keycloak-client-id-2", "Maria Lima", "(11) 99999-0005", "5511999990005");
        db.Clients.AddRange(ana, maria);

        await db.SaveChangesAsync();

        // ─── Appointments ─────────────────────────────────
        var staffService = await db.StaffServices
            .FirstAsync(s => s.StaffId == juliana.Id);

        var hoje = DateTime.UtcNow.Date;

        db.Appointments.AddRange(
            Appointment.Create(branch.Id, juliana.Id, ana.Id, staffService.Id,
                hoje.AddHours(9), "web"),
            Appointment.Create(branch.Id, juliana.Id, maria.Id, staffService.Id,
                hoje.AddHours(10), "whatsapp"),
            Appointment.Create(branch.Id, juliana.Id, ana.Id, staffService.Id,
                hoje.AddHours(14), "mobile"),
            Appointment.Create(branch.Id, juliana.Id, maria.Id, staffService.Id,
                hoje.AddHours(16), "web")
        );

        await db.SaveChangesAsync();
    }
}
