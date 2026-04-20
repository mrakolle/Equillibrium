using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Equillibrium.Manufacturing.Infrastructure.Data;
using Equillibrium.Manufacturing.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Equillibrium.Manufacturing.Controllers;

[ApiController]
[Route("api/manufacturing")]
[Authorize] // Every manufacturing action requires a valid user/tenant
public class ManufacturingController : ControllerBase
{
    private readonly ManufacturingDbContext _context;

    public ManufacturingController(ManufacturingDbContext context)
    {
        _context = context;
    }

    [HttpGet("recipes")]
    public async Task<IActionResult> GetRecipes()
    {
        // Now that 'using Microsoft.EntityFrameworkCore' is at the top, 
        // this will work perfectly.
        var recipes = await _context.Recipes.ToListAsync();
        
        return Ok(recipes);
    }

    [HttpPost("recipes")]
    public async Task<IActionResult> CreateRecipe([FromBody] Recipe recipe)
    {
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
        return Ok(recipe);
    }
}
