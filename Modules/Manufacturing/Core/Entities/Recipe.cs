using Equillibrium.QC.Core.Entities;
namespace Equillibrium.Manufacturing.Core.Entities;

public enum RecipeStatus
{
    Draft,
    Active,
    Superseded,
    Archived
}

public class Recipe
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    
    // Version Control logic
    public string Version { get; set; } = "1.0";
    public RecipeStatus Status { get; set; } = RecipeStatus.Draft;
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    // The "What" (Materials)
    public List<BOMItem> BillOfMaterials { get; set; } = new();
    
    // The "How" (Standard Instructions)
    public List<Instruction> Instructions { get; set; } = new();

    // The "Gate" (QC Targets)
    //public List<Specification> Specifications { get; set; } = new();

    // The "Track" (Default Route)
    public List<RouteStep> DefaultRoute { get; set; } = new();
}
