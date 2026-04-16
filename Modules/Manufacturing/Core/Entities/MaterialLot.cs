namespace Equillibrium.Manufacturing.Core.Entities;

public class MaterialLot
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public string SupplierLotNo { get; set; } = string.Empty; // Full Origin Tracking
    public string InternalBatchNo { get; set; } = string.Empty;
    
    public decimal QuantityReceived { get; set; }
    public decimal QuantityRemaining { get; set; }
    public decimal CostAtPurchase { get; set; } // Specific cost for this specific lot
    
    public DateTime ReceivedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
