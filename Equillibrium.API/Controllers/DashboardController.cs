using Microsoft.AspNetCore.Mvc;
using Equillibrium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Equillibrium.Core.Interfaces;
using Equillibrium.Application.DTOs;

namespace Equillibrium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public DashboardController(ApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet("snapshot")]
    public async Task<ActionResult<TenantDashboardDto>> GetSnapshot()
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) return BadRequest("No Tenant ID found.");

        // 1. Get Tenant Info from Public Catalog
        var tenantInfo = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id.ToString() == tenantId);

        // 2. Count Local Data (Schema-specific)
        var supplierCount = await _context.Contacts.CountAsync();
        var materialCount = await _context.Materials.CountAsync();

        // 3. Count Pending Updates (Items in Public not yet in Tenant Schema)
        // This is a direct check to see if they need to hit that 'Sync' button
        var pendingCount = await _context.GlobalUpdates
            .CountAsync(u => !_context.Contacts.Any(c => c.Id == u.EntityId));

        var dto = new TenantDashboardDto
        {
            TenantName = tenantInfo?.Name ?? "Unknown Tenant",
            LocalSupplierCount = supplierCount,
            LocalMaterialCount = materialCount,
            PendingGlobalUpdates = pendingCount,
            RecentActivity = new List<string> { "System ready", "Schema verified" }
        };

        return Ok(dto);
    }
}
