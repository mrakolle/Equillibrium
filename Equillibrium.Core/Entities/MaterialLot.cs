using System;
using System.Text.Json.Serialization;

namespace Equillibrium.Core.Entities;

public class MaterialLot
{
    public Guid Id { get; set; }
    public Guid MaterialId { get; set; } 

    public string LotNumber { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public virtual Material? Material { get; set; } 
}
