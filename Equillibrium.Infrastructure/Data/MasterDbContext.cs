using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace Equillibrium.Infrastructure.Data;
public class Tenant {
    [Key]
    public Guid TenantId { get; set; }
    public string UniqueSlug { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}
public class MasterDbContext : DbContext {
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }
    public DbSet<Tenant> Tenants => Set<Tenant>();
}
