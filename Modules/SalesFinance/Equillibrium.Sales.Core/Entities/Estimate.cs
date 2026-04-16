namespace Equillibrium.Sales.Core.Entities;

public class Estimate 
{
    public Guid Id { get; set; }
    public string EstimateNumber { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public List<EstimateItem> Items { get; set; } = new();
}