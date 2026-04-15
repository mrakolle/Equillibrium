namespace Equillibrium.Application.DTOs;

public class CreateMaterialDto
{
    public string Name { get; set; } = null!;
    public string? Sku { get; set; }
    public string? Grade { get; set; }
    public string? BaseUnit { get; set; }
    public List<MaterialSupplierLinkDto> Suppliers { get; set; } = new();
}

public class MaterialSupplierLinkDto
{
    public Guid SupplierId { get; set; }
    public string? Price { get; set; } // Keep as string if your entity uses string for price
    public decimal? PurityPercentage { get; set; } // Changed to decimal?
    public string? SdsUrl { get; set; }
    public string? Note { get; set; }
}
