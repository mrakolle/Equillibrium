namespace Equillibrium.Core.Entities;

public class ContactCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // "Raw Materials", "Branding", etc.

    // Navigation property for the Many-to-Many relationship
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}