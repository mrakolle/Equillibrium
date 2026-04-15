using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Equillibrium.Core.Interfaces;
using Equillibrium.Core.Entities;
namespace Equillibrium.Infrastructure.Data;

public class ApplicationDbContext : DbContext 
{
    private readonly ITenantService _tenantService;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService) 
        : base(options) 
    {
        _tenantService = tenantService;
    }

    // --- GLOBAL CATALOG DB SETS ---
    public DbSet<Equillibrium.Core.Entities.Tenant> Tenants => Set<Equillibrium.Core.Entities.Tenant>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<ContactCategory> ContactCategories => Set<ContactCategory>();
    public DbSet<GlobalUpdate> GlobalUpdates => Set<GlobalUpdate>();

    // --- TENANT-SPECIFIC DB SETS ---
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<MaterialLot> MaterialLots => Set<MaterialLot>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<SupplierMaterial> SupplierMaterials => Set<SupplierMaterial>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.Metadata.FindProperty("UpdatedAt") != null)
            {
                entityEntry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }

            if (entityEntry.State == EntityState.Added)
            {
                if (entityEntry.Metadata.FindProperty("CreatedAt") != null)
                {
                    entityEntry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) 
    {
        options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        base.OnModelCreating(modelBuilder);
        
        // --- 1. GLOBAL CATALOG (Public Schema) ---
        modelBuilder.Entity<Equillibrium.Core.Entities.Tenant>(entity =>
        {
            entity.ToTable("Tenants", "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<GlobalUpdate>(entity => 
        {
            entity.ToTable("GlobalUpdates", "public");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contacts", "public");
            entity.HasKey(e => e.Id);
            
            entity.HasMany(c => c.Categories)
                .WithMany(cc => cc.Contacts)
                .UsingEntity(j => j.ToTable("ContactCategoryMappings", "public"));
        });

        modelBuilder.Entity<ContactCategory>(entity =>
        {
            entity.ToTable("ContactCategories", "public");
            entity.HasKey(e => e.Id);

            entity.HasData(
                new ContactCategory { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Raw Materials" },
                new ContactCategory { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Laboratory Supplies" },
                new ContactCategory { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Trainer" },
                new ContactCategory { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Quality Controller" },
                new ContactCategory { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Branding" }
            );
        });
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.ToTable("PurchaseOrders");
            entity.HasKey(e => e.Id);
            
            // Link to the Contact (Supplier) in the same schema
            entity.HasOne(e => e.Supplier)
                .WithMany()
                .HasForeignKey(e => e.SupplierId);
        });

        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.ToTable("PurchaseOrderItems");
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Material)
                .WithMany()
                .HasForeignKey(e => e.MaterialId);
        });

        // --- 2. TENANT-SPECIFIC DATA ---
        modelBuilder.Entity<Material>(entity => 
        {
            entity.ToTable("Materials");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<MaterialLot>(entity =>
        {
            entity.ToTable("MaterialLots");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Material)
                .WithMany(m => m.Lots) 
                .HasForeignKey(e => e.MaterialId)
                .HasPrincipalKey(m => m.Id)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SupplierMaterial>(entity => 
        {
            entity.ToTable("SupplierMaterial"); 
            entity.HasKey(sm => new { sm.SupplierId, sm.MaterialId });

            entity.HasOne(sm => sm.Supplier)
                .WithMany() 
                .HasForeignKey(sm => sm.SupplierId)
                .IsRequired(false); 

            entity.HasOne(sm => sm.Material)
                .WithMany(m => m.SupplierLinks)
                .HasForeignKey(sm => sm.MaterialId);
        });

        modelBuilder.Entity<Invoice>().ToTable("Invoices");
        modelBuilder.Entity<InvoiceItem>().ToTable("InvoiceItems");

        modelBuilder.Entity<Contact>().HasQueryFilter(c => !c.IsDeleted);
    }
}