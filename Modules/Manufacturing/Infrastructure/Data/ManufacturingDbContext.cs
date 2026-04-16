using Microsoft.EntityFrameworkCore;
using Equillibrium.Manufacturing.Core.Entities;
using Equillibrium.QC.Core.Entities;
namespace Equillibrium.Manufacturing.Infrastructure.Data;

public class ManufacturingDbContext : DbContext
{
    public ManufacturingDbContext(DbContextOptions<ManufacturingDbContext> options) : base(options) { }

    // Master Data
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<IngredientSds> IngredientSds => Set<IngredientSds>();
    public DbSet<MaterialLot> MaterialLots => Set<MaterialLot>();
    
    // The Blueprint parts
    public DbSet<BOMItem> BOMItems => Set<BOMItem>();
    public DbSet<Instruction> Instructions => Set<Instruction>();
    //public DbSet<Specification> Specifications => Set<Specification>();
    public DbSet<RouteStep> RouteSteps => Set<RouteStep>();

    // The Execution (The Machine Running)
    public DbSet<BatchRecord> BatchRecords => Set<BatchRecord>();
    public DbSet<BatchRouteProgress> BatchRouteProgress => Set<BatchRouteProgress>();
    public DbSet<BatchMaterialUsage> BatchMaterialUsages => Set<BatchMaterialUsage>();
    //public DbSet<BatchQcResult> BatchQcResults => Set<BatchQcResult>();
    public DbSet<BatchDeviation> BatchDeviations => Set<BatchDeviation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tenants");

        // Many-to-Many: Recipe <-> Ingredient via BOMItem
        modelBuilder.Entity<BOMItem>()
            .HasKey(bi => new { bi.RecipeId, bi.IngredientId });

        // Batch Record Relationships
        modelBuilder.Entity<BatchRecord>(entity =>
        {
            entity.HasMany(b => b.ActualMaterialUsage).WithOne().HasForeignKey(m => m.BatchRecordId);
            entity.HasMany(b => b.RouteProgress).WithOne().HasForeignKey(p => p.BatchRecordId);
            entity.HasMany(b => b.Deviations).WithOne().HasForeignKey(d => d.BatchRecordId);
        });

        // Ensure the SupplierLotNo is indexed for quick trace-backs
        modelBuilder.Entity<MaterialLot>().HasIndex(ml => ml.SupplierLotNo);
        
        base.OnModelCreating(modelBuilder);
    }
}
