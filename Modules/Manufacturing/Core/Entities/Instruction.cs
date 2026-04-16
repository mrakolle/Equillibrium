namespace Equillibrium.Manufacturing.Core.Entities;

public class Instruction
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    
    public int Sequence { get; set; }
    public string Activity { get; set; } = string.Empty; // e.g., "Charge Reactor"
    public string DetailedText { get; set; } = string.Empty; // "Add 20kg of Flour..."
    
    // Optional: Reference to a specific BOM Item for UI highlighting
    public Guid? RelatedBOMItemId { get; set; } 
    
    // Parameters for the machine
    public decimal? TargetValue { get; set; } // e.g., 60.0
    public string? Unit { get; set; } // e.g., "°C"
}
