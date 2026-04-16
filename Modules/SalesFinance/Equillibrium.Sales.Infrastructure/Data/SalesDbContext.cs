using Microsoft.EntityFrameworkCore;
using Equillibrium.Sales.Core.Entities;
using Equillibrium.Core.Entities;

namespace Equillibrium.Sales.Infrastructure.Data;

public class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<Estimate> Estimates => Set<Estimate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Multi-Schema Enforcement
        modelBuilder.HasDefaultSchema("tenants");

        // Example: Configure the Invoice -> Items relationship
        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.Items)
            .WithOne()
            .HasForeignKey(ii => ii.InvoiceId);

        modelBuilder.Entity<Estimate>()
            .HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(ei => ei.EstimateId);

        modelBuilder.Entity<SupplierMaterial>()
            .HasKey(sm => sm.Id); 
        base.OnModelCreating(modelBuilder);
       
    }
}
