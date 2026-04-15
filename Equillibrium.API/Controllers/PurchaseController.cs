using Microsoft.AspNetCore.Mvc;
using Equillibrium.Infrastructure.Data;
using Equillibrium.Core.Entities;
using Equillibrium.Core.Interfaces;
using Equillibrium.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Equillibrium.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public PurchaseController(ApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    [HttpPost("draft")]
    public async Task<IActionResult> CreateDraftOrder([FromBody] CreatePurchaseOrderDto request)
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) return BadRequest("Tenant ID missing.");

        // 1. Verify Supplier exists in THIS tenant's schema
        var supplier = await _context.Contacts.FindAsync(request.SupplierId);
        if (supplier == null) return BadRequest("Supplier not found in your local catalog.");

        // 2. Build the PO Header
        var order = new PurchaseOrder
        {
            Id = Guid.NewGuid(),
            OrderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            SupplierId = request.SupplierId,
            OrderDate = DateTime.UtcNow,
            Status = PurchaseOrderStatus.Draft,
            TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice)
        };

        // 3. Build the Line Items
        foreach (var item in request.Items)
        {
            order.Items.Add(new PurchaseOrderItem
            {
                Id = Guid.NewGuid(),
                PurchaseOrderId = order.Id,
                MaterialId = item.MaterialId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });
        }

        try
        {
            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { 
                Message = "Draft Purchase Order created.", 
                OrderNumber = order.OrderNumber,
                Total = order.TotalAmount 
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to create PO: {ex.Message}");
        }
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _context.PurchaseOrders
            .Include(o => o.Supplier)
            .Include(o => o.Items)
            .ThenInclude(i => i.Material)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Ok(orders);
    }
    [HttpPatch("{id}/confirm")]
    public async Task<IActionResult> ConfirmOrder(Guid id)
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) return BadRequest("Tenant ID missing.");

        // 1. Find the order with its items
        var order = await _context.PurchaseOrders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound("Purchase Order not found.");

        // 2. Only Drafts can be confirmed
        if (order.Status != PurchaseOrderStatus.Draft)
            return BadRequest($"Cannot confirm order in {order.Status} status.");

        // 3. Move to 'Ordered'
        order.Status = PurchaseOrderStatus.Ordered;
        order.OrderDate = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { 
                Message = $"Order {order.OrderNumber} has been placed.", 
                Status = order.Status.ToString() 
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to confirm order: {ex.Message}");
        }
    }
    [HttpPatch("{id}/receive")]
    public async Task<IActionResult> ReceiveOrder(Guid id)
    {
        var tenantId = _tenantService.GetTenantId();
        if (string.IsNullOrEmpty(tenantId)) return BadRequest("Tenant ID missing.");

        // 1. Fetch the order with its line items
        var order = await _context.PurchaseOrders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound("Purchase Order not found.");
        if (order.Status != PurchaseOrderStatus.Ordered)
            return BadRequest("Only orders in 'Ordered' status can be received.");

        // 2. Process each item into the Inventory (MaterialLots)
        foreach (var item in order.Items)
        {
            var newLot = new MaterialLot
            {
                Id = Guid.NewGuid(),
                MaterialId = item.MaterialId,
                Quantity = item.Quantity,
                LotNumber = $"REC-{order.OrderNumber}-{DateTime.UtcNow:ssmm}", // Auto-generated lot ref
                ReceivedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _context.MaterialLots.Add(newLot);
        }

        // 3. Finalise the PO
        order.Status = PurchaseOrderStatus.Received;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { 
                Message = $"Order {order.OrderNumber} received. Inventory has been updated.",
                ItemsReceived = order.Items.Count
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to receive order: {ex.Message}");
        }
    }
}
