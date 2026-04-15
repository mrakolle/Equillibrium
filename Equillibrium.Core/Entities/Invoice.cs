using System.Text.Json.Serialization;

namespace Equillibrium.Core.Entities;

public class Invoice
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public Guid TenantId { get; set; } 

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public Guid ContactId { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Draft"; // e.g., Draft, Sent, Paid, Overdue
    
    // Navigation property for line items
    public List<InvoiceItem> Items { get; set; } = new();
}
