namespace Equillibrium.Core.Entities;

public class GlobalUpdate
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = "Contact"; 
    public Guid EntityId { get; set; } 
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}