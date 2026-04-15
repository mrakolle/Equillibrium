namespace Equillibrium.Application.DTOs;

public class CreatePurchaseOrderDto
{
    public Guid SupplierId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public Guid MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
