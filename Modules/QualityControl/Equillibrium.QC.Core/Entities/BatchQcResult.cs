namespace Equillibrium.QC.Core.Entities;

public class BatchQcResult
{
    public Guid Id { get; set; }
    public Guid BatchRecordId { get; set; }
    
    // Link to the "Target" we defined in the Recipe
    public Guid SpecificationId { get; set; }
    public Specification Specification { get; set; } = null!;

    public decimal MeasuredValue { get; set; }
    public DateTime TestedAt { get; set; } = DateTime.UtcNow;
    public string TestedBy { get; set; } = string.Empty;

    // Logic: Is MeasuredValue between Spec.Min and Spec.Max?
    public bool IsInTolerance { get; set; } 
    public string? Remarks { get; set; }
}
