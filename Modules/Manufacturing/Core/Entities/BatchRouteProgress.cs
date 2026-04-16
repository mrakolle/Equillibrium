namespace Equillibrium.Manufacturing.Core.Entities;

public class BatchRouteProgress
{
    public Guid Id { get; set; }
    public Guid BatchRecordId { get; set; }
    
    public OperationType StepType { get; set; } // Instructions, QC, etc.
    public int Sequence { get; set; }
    
    public bool IsCompleted { get; set; }
    public string? CompletedByUserId { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // If this was a QC step, did it pass?
    public bool? ResultPassed { get; set; }
}
