
using Equillibrium.Core.Enums; 
namespace Equillibrium.QC.Core.Entities;

public class Specification
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }

    public string Parameter { get; set; } = string.Empty; // e.g., "Specific Gravity"
    public string TestMethod { get; set; } = string.Empty; // Instruction for the lab
    
    // The "Pass/Fail" Range
    public decimal MinValue { get; set; }
    public decimal TargetValue { get; set; }
    public decimal MaxValue { get; set; }
    public string Unit { get; set; } = string.Empty; // e.g., "g/cm³"
    
    // Link to Routing: Is this check done during Mixing or Final Packaging?
    public OperationType RouteStage { get; set; } 
}
