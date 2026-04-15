using System;

namespace Equillibrium.Core.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactName { get; set; } 
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<ContactCategory> Categories { get; set; } = new List<ContactCategory>();

}
