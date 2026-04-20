using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Equillibrium.Sales.Infrastructure.Data;
using Equillibrium.Sales.Core.Entities;

namespace Equillibrium.Sales.Controllers;

[ApiController]
[Route("api/sales")]
[Authorize] 
public class SalesController : ControllerBase
{
    private readonly SalesDbContext _context;

    public SalesController(SalesDbContext context)
    {
        _context = context;
    }

    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices()
    {
        var invoices = await _context.Invoices.ToListAsync<Invoice>();
        return Ok(invoices);
    }

    [HttpGet("estimates")]
    public async Task<IActionResult> GetEstimates()
    {
        var estimates = await _context.Estimates.ToListAsync<Estimate>();
        return Ok(estimates);
    }
}
