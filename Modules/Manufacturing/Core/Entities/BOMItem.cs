namespace Equillibrium.Manufacturing.Core.Entities;

public class BOMItem
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}
