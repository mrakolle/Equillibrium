using Microsoft.AspNetCore.Mvc;
using Equillibrium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Equillibrium.Application.DTOs;

namespace Equillibrium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("suppliers/push")]
    public async Task<IActionResult> PushGlobalSupplier([FromBody] GlobalSupplierRequestDto request)
    {
        try
        {
            var categories = await _context.ContactCategories
                .Where(c => request.CategoryIds.Contains(c.Id))
                .ToListAsync();

            var newContact = new Equillibrium.Core.Entities.Contact
            {
                Id = Guid.NewGuid(),
                CompanyName = request.CompanyName,
                ContactName = request.ContactName,
                Email = request.Email,
                Phone = request.Phone,
                Categories = categories,
                CreatedAt = DateTime.UtcNow
            };

            var updateNotification = new Equillibrium.Core.Entities.GlobalUpdate
            {
                Id = Guid.NewGuid(),
                EntityType = "Contact",
                EntityId = newContact.Id,
                Message = $"New {string.Join("/", categories.Select(c => c.Name))} Supplier available: {request.CompanyName}",
                CreatedAt = DateTime.UtcNow
            };

            _context.Contacts.Add(newContact);
            _context.GlobalUpdates.Add(updateNotification);
            
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Global supplier added successfully." });
        }
        catch (Exception ex)
        {
            // DIG OUT THE REAL ERROR
            var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return BadRequest($"Detailed Error: {innerMessage}");
        }
    }
    [HttpPost("materials/push")]
    public async Task<IActionResult> PushGlobalMaterial([FromBody] CreateMaterialDto request)
    {
        var masterMaterialId = Guid.NewGuid();

        var masterMaterial = new Equillibrium.Core.Entities.Material
        {
            Id = masterMaterialId,
            Name = request.Name,
            Sku = request.Sku,
            Grade = request.Grade,
            BaseUnit = request.BaseUnit,
            CreatedAt = DateTime.UtcNow
        };

        // Use the full namespace here to avoid the "not found" error
        foreach (var link in request.Suppliers)
        {
            masterMaterial.SupplierLinks.Add(new Equillibrium.Core.Entities.SupplierMaterial
            {
                MaterialId = masterMaterialId,
                SupplierId = link.SupplierId,
                Price = link.Price,
                PurityPercentage = link.PurityPercentage ?? 0,
                SdsUrl = link.SdsUrl,
                Note = link.Note
            });
        }

        _context.Materials.Add(masterMaterial);
        
        _context.GlobalUpdates.Add(new Equillibrium.Core.Entities.GlobalUpdate {
            Id = Guid.NewGuid(),
            EntityType = "Material",
            EntityId = masterMaterialId,
            Message = $"New Material Template Available: {request.Name}",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Global Material template pushed.", MaterialId = masterMaterialId });
    }

    
}
