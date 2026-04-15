using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Equillibrium.Infrastructure.Data; // <--- MUST HAVE THIS
using Equillibrium.Core.Interfaces;

namespace Equillibrium.Infrastructure;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        var connectionString = "Server=ep-raspy-voice-a1cusl6e-pooler.ap-southeast-1.aws.neon.tech;Database=equilibrium_master;Port=5432;User Id=neondb_owner;Password=npg_syAqizt9nWv6;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;";
        
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options, new DesignTimeTenantService());
    }
}
