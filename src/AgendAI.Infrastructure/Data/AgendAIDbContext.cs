using AgendAI.Core.Domains.Clients;
using AgendAI.Core.Domains.Finance;
using AgendAI.Core.Domains.Scheduling;
using AgendAI.Core.Domains.Services;
using AgendAI.Core.Domains.Staff;
using AgendAI.Core.Domains.Tenants;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infrastructure.Data;

public class AgendAIDbContext : DbContext
{
    public AgendAIDbContext(DbContextOptions<AgendAIDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<StaffMember> Staff => Set<StaffMember>();
    public DbSet<StaffService> StaffServices => Set<StaffService>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ServiceCatalog> ServiceCatalog => Set<ServiceCatalog>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Commission> Commissions => Set<Commission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tenant
        modelBuilder.Entity<Tenant>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Slug).IsUnique();
        });

        // Branch
        modelBuilder.Entity<Branch>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Address).HasMaxLength(255);
            e.Property(x => x.Phone).HasMaxLength(20);
            e.HasIndex(x => x.TenantId);
        });

        // StaffMember
        modelBuilder.Entity<StaffMember>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(20);
            e.Property(x => x.Specialty).HasMaxLength(50);
            e.Property(x => x.KeycloakUserId).HasMaxLength(100);
            e.Property(x => x.CommissionPct).HasPrecision(5, 2);
            e.HasIndex(x => x.BranchId);
            e.HasIndex(x => x.KeycloakUserId);
        });

        // StaffService
        modelBuilder.Entity<StaffService>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Price).HasPrecision(10, 2);
            e.HasIndex(x => new { x.StaffId, x.ServiceCatalogId }).IsUnique();
        });

        // Client
        modelBuilder.Entity<Client>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(20);
            e.Property(x => x.WhatsApp).HasMaxLength(20);
            e.Property(x => x.KeycloakUserId).HasMaxLength(100);
            e.HasIndex(x => x.KeycloakUserId);
            e.HasIndex(x => x.WhatsApp);
        });

        // ServiceCatalog
        modelBuilder.Entity<ServiceCatalog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(255);
        });

        // Appointment
        modelBuilder.Entity<Appointment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.CreatedVia).HasMaxLength(20);
            e.HasIndex(x => new { x.BranchId, x.ScheduledAt });
            e.HasIndex(x => x.StaffId);
            e.HasIndex(x => x.ClientId);
        });

        // Commission
        modelBuilder.Entity<Commission>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasPrecision(10, 2);
            e.Property(x => x.Percentage).HasPrecision(5, 2);
            e.Property(x => x.Period).HasMaxLength(7);
            e.HasIndex(x => new { x.StaffId, x.Period });
            e.HasIndex(x => x.AppointmentId).IsUnique();
        });
    }
}
