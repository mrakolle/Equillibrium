namespace Equillibrium.Manufacturing.Core.Entities;

public class Ingredient
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty; // e.g., kg, L, units
    public decimal MinimumStockLevel { get; set; }
}
