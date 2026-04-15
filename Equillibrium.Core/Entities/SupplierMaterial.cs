using System;

namespace Equillibrium.Core.Entities;

public class SupplierMaterial
{
    public Guid SupplierId { get; set; }
    public Contact? Supplier { get; set; } // Now it sees 'Contact' in the same namespace

    public Guid MaterialId { get; set; }
    public Material? Material { get; set; }

    public decimal PurityPercentage { get; set; }
    public string? SdsUrl { get; set; }
    public string? Price { get; set; }
    public string? Note { get; set; }
}
