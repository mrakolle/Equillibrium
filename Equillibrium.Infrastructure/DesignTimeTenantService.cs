using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Equillibrium.Infrastructure.Data; // <--- MUST HAVE THIS
using Equillibrium.Core.Interfaces;

namespace Equillibrium.Infrastructure;
public class DesignTimeTenantService : ITenantService
{
    // During migrations/design-time, we return null or a default schema
    public string? GetTenantId() => null;
}
