using System;
using System.Collections.Generic;

namespace Equillibrium.Core.Entities;

public class Material
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string? CasNumber { get; set; }
    public string? Grade { get; set; }
    public string? BaseUnit { get; set; }
    
    // Required for Catalog Search/Inventory logic
    public decimal CurrentStock { get; set; }
    public decimal MinimumStockLevel { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<SupplierMaterial> SupplierLinks { get; set; } = new List<SupplierMaterial>();
    public virtual ICollection<MaterialLot> Lots { get; set; } = new List<MaterialLot>();
}
