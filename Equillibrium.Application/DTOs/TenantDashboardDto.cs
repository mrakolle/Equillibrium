namespace Equillibrium.Application.DTOs;

public class TenantDashboardDto
{
    public string TenantName { get; set; } = string.Empty;
    public int LocalSupplierCount { get; set; }
    public int LocalMaterialCount { get; set; }
    public int PendingGlobalUpdates { get; set; }
    public List<string> RecentActivity { get; set; } = new();
}
