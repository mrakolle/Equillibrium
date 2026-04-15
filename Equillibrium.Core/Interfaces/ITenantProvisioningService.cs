namespace Equillibrium.Core.Interfaces;

public interface ITenantProvisioningService
{
    Task ProvisionTenantAsync(string tenantId);
}
