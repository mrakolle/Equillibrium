namespace Equillibrium.Core.Entities;

public class PurchaseOrderItem
{
    public Guid Id { get; set; }
    public Guid PurchaseOrderId { get; set; }
    
    public Guid MaterialId { get; set; }
    public virtual Material Material { get; set; } = null!;
    
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}
