using Microsoft.AspNetCore.Mvc;
using Equillibrium.Core.Interfaces;
using Equillibrium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Equillibrium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public ImportController(ApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    /*[HttpPost("sync-all-suppliers")]
    public async Task<IActionResult> SyncAllSuppliers()
    {
        var tenantId = _tenantService.GetTenantId();
        
        if (string.IsNullOrEmpty(tenantId)) 
            return BadRequest("Tenant ID not found. Use the Authorize button at the top.");

        try 
        {
            await _context.Database.OpenConnectionAsync();

            // FIXED: Removed the 't' from CompanytName -> CompanyName
            var sql = $@"
                INSERT INTO ""{tenantId}"".""Contacts"" (""Id"", ""CompanyName"", ""ContactName"", ""Email"", ""Phone"", ""IsDeleted"", ""CreatedAt"")
                SELECT p.""Id"", p.""CompanyName"", p.""ContactName"", p.""Email"", p.""Phone"", p.""IsDeleted"", NOW()
                FROM public.""Contacts"" p
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""{tenantId}"".""Contacts"" t 
                    WHERE t.""Id"" = p.""Id""
                );";

            int rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql);
            
            return Ok(new { Message = "Sync successful.", NewSuppliers = rowsAffected });
        }
        catch (Exception ex)
        {
            return BadRequest($"Sync failed: {ex.Message}");
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }*/
    [HttpPost("sync-all")]
    public async Task<IActionResult> SyncEverything()
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) return BadRequest("Tenant ID missing.");

        try 
        {
            await _context.Database.OpenConnectionAsync();

            // A. Sync Suppliers (Contacts)
            var syncSuppliers = $@"
                INSERT INTO ""{tenantId}"".""Contacts"" (""Id"", ""CompanyName"", ""ContactName"", ""Email"", ""Phone"", ""IsDeleted"", ""CreatedAt"")
                SELECT p.""Id"", p.""CompanyName"", p.""ContactName"", p.""Email"", p.""Phone"", p.""IsDeleted"", NOW()
                FROM public.""Contacts"" p
                WHERE NOT EXISTS (SELECT 1 FROM ""{tenantId}"".""Contacts"" t WHERE t.""Id"" = p.""Id"");";

            // B. Sync Materials
            var syncMaterials = $@"
                INSERT INTO ""{tenantId}"".""Materials"" (""Id"", ""Name"", ""Sku"", ""Grade"", ""BaseUnit"", ""CreatedAt"")
                SELECT p.""Id"", p.""Name"", p.""Sku"", p.""Grade"", p.""BaseUnit"", NOW()
                FROM public.""Materials"" p
                WHERE NOT EXISTS (SELECT 1 FROM ""{tenantId}"".""Materials"" t WHERE t.""Id"" = p.""Id"");";

            // C. Sync SupplierMaterial Links
            var syncLinks = $@"
                INSERT INTO ""{tenantId}"".""SupplierMaterial"" (""MaterialId"", ""SupplierId"", ""Price"", ""PurityPercentage"", ""SdsUrl"", ""Note"")
                SELECT p.""MaterialId"", p.""SupplierId"", p.""Price"", p.""PurityPercentage"", p.""SdsUrl"", p.""Note""
                FROM public.""SupplierMaterial"" p
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""{tenantId}"".""SupplierMaterial"" t 
                    WHERE t.""MaterialId"" = p.""MaterialId"" AND t.""SupplierId"" = p.""SupplierId""
                );";

            await _context.Database.ExecuteSqlRawAsync(syncSuppliers);
            await _context.Database.ExecuteSqlRawAsync(syncMaterials);
            await _context.Database.ExecuteSqlRawAsync(syncLinks);

            return Ok(new { Message = "Tenant schema is now fully synced with Global Catalog." });
        }
        finally { await _context.Database.CloseConnectionAsync(); }
    }

}
