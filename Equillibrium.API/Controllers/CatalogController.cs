using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Equillibrium.Infrastructure.Data;
using Equillibrium.Core.Interfaces;
using Equillibrium.Core.Entities;
using Equillibrium.Application.DTOs;
using System.Globalization;

namespace Equillibrium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public CatalogController(ApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpPost("materials")]
    public async Task<IActionResult> AddMaterial([FromBody] CreateMaterialDto request)
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) return BadRequest("Tenant ID missing. Use the Authorize button.");

        var material = new Material
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Sku = request.Sku,
            Grade = request.Grade,
            BaseUnit = request.BaseUnit,
            CreatedAt = DateTime.UtcNow
        };

        // Link the material to each provided supplier with specific commerce details
        foreach (var link in request.Suppliers)
        {
            material.SupplierLinks.Add(new SupplierMaterial
            {
                MaterialId = material.Id,
                SupplierId = link.SupplierId,
                Price = link.Price,
                // The '?? 0' says: If PurityPercentage is null, use 0 instead.
                PurityPercentage = link.PurityPercentage ?? 0, 
                SdsUrl = link.SdsUrl,
                Note = link.Note
            });
        }

        try
        {
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Material added to catalog.", MaterialId = material.Id });
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to add material: {ex.Message}");
        }
    }

    [HttpGet("portal-search")]
    public async Task<IActionResult> PortalSearch([FromQuery] string? name)
    {
        var tenantId = _tenantService.GetTenantId();
        bool isRegistered = !string.IsNullOrEmpty(tenantId);

        var materials = await _context.Materials
            .Include(m => m.SupplierLinks)
                .ThenInclude(sl => sl.Supplier)
            .AsNoTracking()
            .Where(m => string.IsNullOrEmpty(name) || m.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();

        var result = materials.Select(m => new
        {
            m.Name,
            m.Sku,
            m.Grade,
            m.BaseUnit,
            Suppliers = m.SupplierLinks.Select(sl => new
            {
                CompanyName = sl.Supplier?.CompanyName ?? "Unknown",
                Purity = sl.PurityPercentage,
                SdsUrl = sl.SdsUrl,
                Price = isRegistered ? FormatPrice(sl.Price) : "REDACTED",
                Note = sl.Note
            })
        });

        return Ok(result);
    }

    private static string FormatPrice(string? price)
    {
        if (string.IsNullOrWhiteSpace(price)) return "R 0.00";
        if (decimal.TryParse(price, out var numericPrice))
        {
            return numericPrice.ToString("C", CultureInfo.GetCultureInfo("en-ZA"));
        }
        return price;
    }
    [HttpGet("discovery")]
    public async Task<IActionResult> GlobalDiscovery([FromQuery] string? searchTerm)
    {
        // We query the Global Contacts (Suppliers) in the 'public' schema
        var globalSuppliers = await _context.Contacts
            .Where(c => string.IsNullOrEmpty(searchTerm) || 
                        c.CompanyName.ToLower().Contains(searchTerm.ToLower()))
            .Select(c => new {
                c.Id,
                c.CompanyName,
                c.ContactName,
                c.Email,
                Categories = c.Categories.Select(cat => cat.Name)
            })
            .ToListAsync();

        return Ok(new {
            Description = "Master Catalog of CHEM-IQ Verified Suppliers",
            AvailableSuppliers = globalSuppliers
        });
    }

}
