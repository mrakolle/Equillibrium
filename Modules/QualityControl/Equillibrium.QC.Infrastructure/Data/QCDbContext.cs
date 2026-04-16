using Microsoft.EntityFrameworkCore;
using Equillibrium.QC.Core.Entities;

namespace Equillibrium.QC.Infrastructure.Data;

public class QCDbContext : DbContext
{
    public QCDbContext(DbContextOptions<QCDbContext> options) : base(options) { }

    public DbSet<Specification> Specifications => Set<Specification>();
    public DbSet<BatchQcResult> BatchQcResults => Set<BatchQcResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tenants");
        base.OnModelCreating(modelBuilder);
    }
}
