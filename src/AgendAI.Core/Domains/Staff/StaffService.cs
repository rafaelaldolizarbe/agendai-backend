namespace AgendAI.Core.Domains.Staff;

public class StaffService
{
    public Guid Id { get; private set; }
    public Guid StaffId { get; private set; }
    public Guid ServiceCatalogId { get; private set; }
    public decimal Price { get; private set; }
    public bool Active { get; private set; }

    private StaffService() { }

    public static StaffService Create(Guid staffId, Guid serviceCatalogId, decimal price)
    {
        return new StaffService
        {
            Id = Guid.NewGuid(),
            StaffId = staffId,
            ServiceCatalogId = serviceCatalogId,
            Price = price,
            Active = true
        };
    }

    public void UpdatePrice(decimal newPrice) => Price = newPrice;
    public void Deactivate() => Active = false;
}
