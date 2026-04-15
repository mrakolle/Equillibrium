using System.Text.Json.Serialization;

namespace Equillibrium.Core.Entities;

public class InvoiceItem
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public Guid TenantId { get; set; } 

    public Guid InvoiceId { get; set; }

    public Guid MaterialId { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    [JsonIgnore] // Calculated property; not stored in the database
    public decimal TotalPrice => Quantity * UnitPrice;
}
