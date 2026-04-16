namespace Equillibrium.Manufacturing.Core.Entities;

public enum OperationType
{
    Instructions, // Standard Production
    QualityControl, // Lab/Testing
    Packaging,    // Finalizing
    Logistics     // Shipping/Labelling
}

public class RouteStep
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    
    public int Sequence { get; set; } // 1, 2, 3...
    public OperationType Type { get; set; } 
    public string Description { get; set; } = string.Empty; // e.g., "Standard Mixing"
    
    public bool RequiresSignOff { get; set; } // Stops the batch until approved
}
