namespace Equillibrium.Application.DTOs;

public class GlobalSupplierRequestDto
{
    public string CompanyName { get; set; } = null!;
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    
    // List of Category IDs (e.g., the Guid for 'Chemicals')
    public List<Guid> CategoryIds { get; set; } = new List<Guid>();
}