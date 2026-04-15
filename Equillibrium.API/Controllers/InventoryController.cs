using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Equillibrium.Infrastructure.Data;
using Equillibrium.Core.Interfaces;
using Equillibrium.Core.Entities;

namespace Equillibrium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ApplicationDbContext _context;

    public InventoryController(ITenantService tenantService, ApplicationDbContext context)
    {
        _tenantService = tenantService;
        _context = context;
    }

    [HttpGet("lots")]
    public async Task<ActionResult<IEnumerable<MaterialLot>>> GetMaterialLots()
    {
        try
        {
            // The Interceptor handles the schema switching automatically now
            var result = await _context.MaterialLots
                .Include(m => m.Material)
                .ToListAsync();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error fetching lots: {ex.Message}");
        }
    }
    [HttpGet("stock-summary")]
    public async Task<IActionResult> GetStockSummary()
    {
        var stock = await _context.MaterialLots
            .Include(l => l.Material)
            .GroupBy(l => new { 
                l.MaterialId, 
                // Use ?? to provide a fallback name if Material is null
                Name = l.Material != null ? l.Material.Name : "Unknown Material", 
                Unit = l.Material != null ? l.Material.BaseUnit : "N/A" 
            })
            .Select(g => new
            {
                MaterialName = g.Key.Name,
                Unit = g.Key.Unit,
                TotalQuantity = g.Sum(l => l.Quantity),
                LotCount = g.Count(),
                Batches = g.Select(l => new { 
                    l.LotNumber, 
                    l.Quantity, 
                    l.ReceivedDate 
                })
            })
            .ToListAsync();

        return Ok(stock);
    }

}