using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Equillibrium.QC.Infrastructure.Data;
using Equillibrium.QC.Core.Entities;

namespace Equillibrium.QC.Controllers;

[ApiController]
[Route("api/qc")]
[Authorize]
public class QCController : ControllerBase
{
    private readonly QCDbContext _context;

    public QCController(QCDbContext context)
    {
        _context = context;
    }

    [HttpGet("specs")]
    public async Task<IActionResult> GetSpecifications()
    {
        var specs = await _context.Specifications.ToListAsync<Specification>();
        return Ok(specs);
    }
}
