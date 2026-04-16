namespace Equillibrium.Manufacturing.Core.Entities;

public class BatchDeviation
{
    public Guid Id { get; set; }
    public Guid BatchRecordId { get; set; }
    
    public DateTime OccurredAt { get; set; }
    public string Description { get; set; } = string.Empty; // "Power outage during mixing"
    public string ImpactAssessment { get; set; } = string.Empty; // "Batch temp dropped 5 degrees"
    
    public bool RequiresManagementSignOff { get; set; }
    public string? Resolution { get; set; } // "Continue after heating back to 60C"
    public string LoggedByUserId { get; set; } = string.Empty;
}
