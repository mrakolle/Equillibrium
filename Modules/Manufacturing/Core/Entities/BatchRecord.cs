namespace Equillibrium.Manufacturing.Core.Entities;
public enum BatchStatus
{
    Scheduled,
    InProgress,
    InQualityControl,
    AwaitingApproval,
    Released,
    Rejected
}
public class BatchRecord
{
    public Guid Id { get; set; }
    public string BatchNumber { get; set; } = string.Empty; // e.g. B-20240416-001
     // The physical ingredients consumed
    public List<BatchMaterialUsage> ActualMaterialUsage { get; set; } = new();
    // Link to the Blueprint
    public Guid RecipeId { get; set; }
    public string RecipeVersionAtSnapshot { get; set; } = string.Empty;
    
    // Status & Timing
    public BatchStatus Status { get; set; } = BatchStatus.Scheduled;
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }

    // Routing Execution
    // This tracks which RouteSteps have been completed
    public List<BatchRouteProgress> RouteProgress { get; set; } = new();

    // Final Approval (The Digital Signature)
    public bool IsApproved { get; set; }
    public string? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalComments { get; set; }

    // Financial/Yield Outcome
    public decimal PlannedQuantity { get; set; }
    public decimal ActualYield { get; set; }
    // The exceptions encountered
    public List<BatchDeviation> Deviations { get; set; } = new();
    public decimal CalculateVariance()
    {
        var totalInput = ActualMaterialUsage.Sum(m => m.QuantityUsed);
        return totalInput - ActualYield;
    }
    }
