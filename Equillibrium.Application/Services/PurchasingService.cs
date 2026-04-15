using Equillibrium.Application.Interfaces;
using Equillibrium.Application.DTOs;
namespace Equillibrium.Application.Services;
public class PurchasingService : IPurchasingService {
    public Task ProcessOrderAsync() => Task.CompletedTask;
}
