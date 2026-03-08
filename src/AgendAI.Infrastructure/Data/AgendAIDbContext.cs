using AgendAI.Core.Domains.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace AgendAI.Infrastructure.Data;

public class AgendAIDbContext : DbContext
{
    public AgendAIDbContext(DbContextOptions<AgendAIDbContext> options) : base(options) { }

    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Appointment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Service).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedVia).HasMaxLength(20);
            e.HasIndex(x => new { x.TenantId, x.ScheduledAt });
            e.HasIndex(x => x.StaffId);
        });
    }
}