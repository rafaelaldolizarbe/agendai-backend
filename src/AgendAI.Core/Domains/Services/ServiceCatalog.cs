namespace AgendAI.Core.Domains.Services;

public class ServiceCatalog
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int DurationMinutes { get; private set; }

    private ServiceCatalog() { }

    public static ServiceCatalog Create(string name, string description, int durationMinutes)
    {
        return new ServiceCatalog
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            DurationMinutes = durationMinutes
        };
    }
}
