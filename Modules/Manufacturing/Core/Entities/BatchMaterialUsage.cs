namespace Equillibrium.Manufacturing.Core.Entities;

public class BatchMaterialUsage
{
    public Guid Id { get; set; }
    public Guid BatchRecordId { get; set; }
    
    // Link to the specific batch of raw material
    public Guid MaterialLotId { get; set; }
    public MaterialLot MaterialLot { get; set; } = null!;

    public decimal QuantityUsed { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    public string? ScannedBy { get; set; } // User who scanned the barcode/lot
}
