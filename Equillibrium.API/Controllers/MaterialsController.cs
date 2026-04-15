using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Equillibrium.Infrastructure.Data;
using Equillibrium.Core.Entities;

namespace Equillibrium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MaterialsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Materials (Now includes Lots for full visibility)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Material>>> GetMaterials()
    {
        return await _context.Materials
            .Include(m => m.Lots)
            .ToListAsync();
    }

    // POST: api/Materials
    [HttpPost]
    public async Task<ActionResult<Material>> CreateMaterial(Material material)
    {
        material.Id = Guid.NewGuid();
        _context.Materials.Add(material);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMaterials), new { id = material.Id }, material);
    }

    // NEW: POST api/Materials/{id}/lots
    [HttpPost("{id}/lots")]
    public async Task<ActionResult<MaterialLot>> AddLot(Guid id, MaterialLot lot)
    {
        var materialExists = await _context.Materials.AnyAsync(m => m.Id == id);
        if (!materialExists) return NotFound("Chemical Material not found.");

        lot.Id = Guid.NewGuid();
        lot.MaterialId = id;
        
        _context.MaterialLots.Add(lot);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMaterials), new { id = lot.Id }, lot);
    }
}
