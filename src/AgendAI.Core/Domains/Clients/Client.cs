namespace AgendAI.Core.Domains.Clients;

public class Client
{
    public Guid Id { get; private set; }
    public string KeycloakUserId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string WhatsApp { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Client() { }

    public static Client Create(
        string keycloakUserId,
        string name,
        string phone,
        string whatsApp)
    {
        return new Client
        {
            Id = Guid.NewGuid(),
            KeycloakUserId = keycloakUserId,
            Name = name,
            Phone = phone,
            WhatsApp = whatsApp,
            CreatedAt = DateTime.UtcNow
        };
    }
}
