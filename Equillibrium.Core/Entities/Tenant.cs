namespace Equillibrium.Core.Entities;

public class Tenant
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Telephone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
