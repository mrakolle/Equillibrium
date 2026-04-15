using Equillibrium.Application.Interfaces;
using Equillibrium.Application.DTOs;
namespace Equillibrium.Application.Services;
public class InvoicingService : IInvoicingService 
{
    public Task CreateEstimateAsync(InvoiceDto dto) => Task.CompletedTask;
    
}
