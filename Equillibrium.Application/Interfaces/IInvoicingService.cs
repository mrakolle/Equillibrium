using Equillibrium.Application.DTOs;
namespace Equillibrium.Application.Interfaces;
public interface IInvoicingService {
    Task CreateEstimateAsync(InvoiceDto dto);
}
