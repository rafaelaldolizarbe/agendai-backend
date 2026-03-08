namespace AgendAI.Core.Domains.Scheduling;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid StaffId { get; private set; }
    public Guid TenantId { get; private set; }
    public string Service { get; private set; } = string.Empty;
    public DateTime ScheduledAt { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? CreatedVia { get; private set; }

    private Appointment() { }

    public static Appointment Create(
        Guid clientId,
        Guid staffId,
        Guid tenantId,
        string service,
        DateTime scheduledAt,
        string createdVia = "web")
    {
        return new Appointment
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            StaffId = staffId,
            TenantId = tenantId,
            Service = service,
            ScheduledAt = DateTime.SpecifyKind(scheduledAt, DateTimeKind.Utc),
            Status = AppointmentStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            CreatedVia = createdVia
        };
    }

    public void Reschedule(DateTime newDate)
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Não é possível reagendar um agendamento cancelado.");

        ScheduledAt = DateTime.SpecifyKind(newDate, DateTimeKind.Utc);
    }

    public void Cancel()
    {
        Status = AppointmentStatus.Cancelled;
    }
}

public enum AppointmentStatus
{
    Confirmed,
    Completed,
    Cancelled,
    NoShow
}
