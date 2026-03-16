namespace AgendAI.Core.Domains.Tenants;

public class Branch
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public bool Active { get; private set; }

    private Branch() { }

    public static Branch Create(Guid tenantId, string name, string address, string phone)
    {
        return new Branch
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            Address = address,
            Phone = phone,
            Active = true
        };
    }

    public void Deactivate() => Active = false;
    public void Activate() => Active = true;
}
