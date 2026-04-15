namespace Equillibrium.Application.DTOs;

public class OnboardRequestDto 
{
    public string TenantName { get; set; } = null!;
    public string? Address { get; set; }
    public string? Telephone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? LogoUrl { get; set; }
}
