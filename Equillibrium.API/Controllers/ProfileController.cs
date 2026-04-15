using Microsoft.AspNetCore.Mvc;
using Equillibrium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Equillibrium.Core.Interfaces;
using Equillibrium.Application.DTOs;

namespace Equillibrium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public ProfileController(ApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) return BadRequest("Tenant ID missing.");

        // Pull from the public Tenants table
        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id.ToString() == tenantId);

        if (tenant == null) return NotFound("Tenant profile not found.");

        return Ok(tenant);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateTenantProfileDto request)
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) return BadRequest("Tenant ID missing.");

        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id.ToString() == tenantId);

        if (tenant == null) return NotFound("Tenant profile not found.");

        // Update fields
        tenant.Address = request.Address;
        tenant.Telephone = request.Telephone;
        tenant.Mobile = request.Mobile;
        tenant.Email = request.Email;
        tenant.LogoUrl = request.LogoUrl;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Profile updated successfully.", Tenant = tenant.Name });
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to update profile: {ex.Message}");
        }
    }
}
