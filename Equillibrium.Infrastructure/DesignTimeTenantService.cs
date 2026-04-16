using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Equillibrium.Infrastructure.Data; // 
using Equillibrium.Core.Interfaces;

namespace Equillibrium.Infrastructure;
public class DesignTimeTenantService : ITenantService
{
    // During migrations/design-time, we return null or a default schema
    public string? GetTenantId() => null;
    public string GetTenantSchema()
    {
        // For design-time (migrations), we usually target a "template" or "public" schema
        return "tenants"; 
    }

}
