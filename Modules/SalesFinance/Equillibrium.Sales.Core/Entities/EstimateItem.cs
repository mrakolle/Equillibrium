namespace Equillibrium.Sales.Core.Entities;

public class EstimateItem 
{
    public Guid Id { get; set; }
    public Guid EstimateId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}