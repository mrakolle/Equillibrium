namespace Equillibrium.Manufacturing.Core.Entities;

public class IngredientSds
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    
    public string HazardStatements { get; set; } = string.Empty;
    public string SignalWord { get; set; } = "Warning"; // e.g., Warning, Danger
    
    public string DocumentUrl { get; set; } = string.Empty; // Path to PDF
    public DateTime LastReviewed { get; set; }
}
