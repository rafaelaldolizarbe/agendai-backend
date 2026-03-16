namespace AgendAI.Core.Domains.Scheduling;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid StaffId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid StaffServiceId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? CreatedVia { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Appointment() { }

    public static Appointment Create(
        Guid branchId,
        Guid staffId,
        Guid clientId,
        Guid staffServiceId,
        DateTime scheduledAt,
        string createdVia = "web")
    {
        return new Appointment
        {
            Id = Guid.NewGuid(),
            BranchId = branchId,
            StaffId = staffId,
            ClientId = clientId,
            StaffServiceId = staffServiceId,
            ScheduledAt = DateTime.SpecifyKind(scheduledAt, DateTimeKind.Utc),
            Status = AppointmentStatus.Confirmed,
            CreatedVia = createdVia,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Reschedule(DateTime newDate)
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Não é possível reagendar um agendamento cancelado.");

        ScheduledAt = DateTime.SpecifyKind(newDate, DateTimeKind.Utc);
    }

    public void Complete() => Status = AppointmentStatus.Completed;
    public void Cancel() => Status = AppointmentStatus.Cancelled;
    public void MarkNoShow() => Status = AppointmentStatus.NoShow;
}

public enum AppointmentStatus
{
    Confirmed,
    Completed,
    Cancelled,
    NoShow
}
