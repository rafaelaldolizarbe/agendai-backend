namespace AgendAI.Core.Domains.Tenants;

public class Tenant
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ICollection<Branch> Branches { get; private set; } = [];

    private Tenant() { }

    public static Tenant Create(string name, string slug)
    {
        return new Tenant
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug.ToLower().Replace(" ", "-"),
            Active = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate() => Active = false;
    public void Activate() => Active = true;
}
