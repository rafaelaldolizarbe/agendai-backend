namespace AgendAI.Core.Domains.Finance;

public class Commission
{
    public Guid Id { get; private set; }
    public Guid StaffId { get; private set; }
    public Guid AppointmentId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal Percentage { get; private set; }
    public string Period { get; private set; } = string.Empty;
    public CommissionStatus Status { get; private set; }

    private Commission() { }

    public static Commission Create(
        Guid staffId,
        Guid appointmentId,
        decimal servicePrice,
        decimal percentage)
    {
        return new Commission
        {
            Id = Guid.NewGuid(),
            StaffId = staffId,
            AppointmentId = appointmentId,
            Amount = Math.Round(servicePrice * percentage / 100, 2),
            Percentage = percentage,
            Period = DateTime.UtcNow.ToString("yyyy-MM"),
            Status = CommissionStatus.Pending
        };
    }

    public void MarkAsPaid() => Status = CommissionStatus.Paid;
}

public enum CommissionStatus
{
    Pending,
    Paid
}
