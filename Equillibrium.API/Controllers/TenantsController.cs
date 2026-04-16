using Microsoft.AspNetCore.Mvc;
using Equillibrium.Core.Interfaces;
using Equillibrium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Equillibrium.Application.DTOs;

namespace Equillibrium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantProvisioningService _provisioningService;
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public TenantsController(
        ITenantProvisioningService provisioningService, 
        ApplicationDbContext context,
        ITenantService tenantService)
    {
        _provisioningService = provisioningService;
        _context = context;
        _tenantService = tenantService;
    }

    [HttpPost("onboard")]
    public async Task<IActionResult> OnboardTenant([FromBody] OnboardRequestDto request)
    {
        var existing = await _context.Tenants
            .IgnoreQueryFilters() 
            .FirstOrDefaultAsync(t => t.Name.ToLower() == request.TenantName.ToLower());

        if (existing != null) return BadRequest("Tenant name already exists.");

        var newTenant = new Equillibrium.Core.Entities.Tenant 
        { 
            Id = Guid.NewGuid(), 
            Name = request.TenantName,
            Address = request.Address,
            Telephone = request.Telephone,
            Mobile = request.Mobile,
            Email = request.Email,
            LogoUrl = request.LogoUrl
        };

        try 
        {
            await _provisioningService.ProvisionTenantAsync(newTenant.Id.ToString());
            _context.Tenants.Add(newTenant);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Successfully onboarded {request.TenantName}", TenantId = newTenant.Id });
        }
        catch (Exception ex)
        {
            return BadRequest($"Provisioning failed: {ex.Message}");
        }
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications()
    {
        // Fetches notifications from the public.GlobalUpdates table
        var notifications = await _context.GlobalUpdates
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        // Ensure this return is outside the logic but inside the method
        return Ok(notifications); 
    }


    /*[HttpPost("import-supplier/{globalContactId}")]
    public async Task<IActionResult> ImportSupplier(Guid globalContactId)
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) return BadRequest("Tenant ID not found in session.");

        try 
        {
            // Raw SQL to clone the specific contact from public to the current tenant's schema
            var sql = $@"
                INSERT INTO ""{tenantId}"".""Contacts"" (""Id"", ""CompanyName"", ""ContactName"", ""Email"", ""Phone"", ""IsDeleted"", ""CreatedAt"")
                SELECT ""Id"", ""CompanyName"", ""ContactName"", ""Email"", ""Phone"", ""IsDeleted"", NOW()
                FROM public.""Contacts""
                WHERE ""Id"" = '{globalContactId}'
                ON CONFLICT (""Id"") DO NOTHING;";

            await _context.Database.ExecuteSqlRawAsync(sql);
            return Ok(new { Message = "Supplier successfully imported to your private catalog." });
        }
        catch (Exception ex)
        {
            return BadRequest($"Import failed: {ex.Message}");
        }
    }*/
}
