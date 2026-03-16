namespace AgendAI.Core.Domains.Staff;

public class StaffMember
{
    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public string KeycloakUserId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Specialty { get; private set; } = string.Empty;
    public decimal CommissionPct { get; private set; }
    public bool Active { get; private set; }
    public ICollection<StaffService> Services { get; private set; } = [];

    private StaffMember() { }

    public static StaffMember Create(
        Guid branchId,
        string keycloakUserId,
        string name,
        string phone,
        string specialty,
        decimal commissionPct)
    {
        return new StaffMember
        {
            Id = Guid.NewGuid(),
            BranchId = branchId,
            KeycloakUserId = keycloakUserId,
            Name = name,
            Phone = phone,
            Specialty = specialty,
            CommissionPct = commissionPct,
            Active = true
        };
    }

    public void Deactivate() => Active = false;
    public void Activate() => Active = true;
    public void UpdateCommission(decimal newPct) => CommissionPct = newPct;
}
