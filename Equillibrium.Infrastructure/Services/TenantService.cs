using System.Security.Claims;
using Equillibrium.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Equillibrium.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetTenantId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return null;

        // 1. Try Header first (standard for development/swagger)
        var tenantId = context.Request.Headers["X-Tenant-Id"].ToString();

        // 2. Fallback to JWT Claims (Production logic)
        if (string.IsNullOrEmpty(tenantId))
        {
            // Looks for the 'TenantId' claim inside your Bearer token
            tenantId = context.User?.FindFirstValue("TenantId");
        }

        return string.IsNullOrEmpty(tenantId) ? null : tenantId;
    }
    public string GetTenantSchema()
    {
        // Example: Get from X-Tenant-Id header and format as "tenant_uuid"
        var tenantId = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].ToString();
        
        if (string.IsNullOrEmpty(tenantId))
            return "public"; // Fallback to public if no tenant provided

        return $"tenant_{tenantId.Replace("-", "_")}";
    }
}
