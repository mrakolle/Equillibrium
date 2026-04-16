using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Equillibrium.Infrastructure.Data.Persistence;
using Equillibrium.Manufacturing.Infrastructure.Data;
using Equillibrium.Sales.Infrastructure.Data;
using Equillibrium.QC.Infrastructure.Data;

public static class ModuleRegistration
{
    public static IServiceCollection AddEcosystemModules(this IServiceCollection services, string connectionString)
    {
        // Register the Interceptor as Scoped so it gets the current Tenant from ITenantService
        services.AddScoped<TenantSchemaInterceptor>();

        // Register Manufacturing
        services.AddDbContext<ManufacturingDbContext>((sp, options) => {
            options.UseNpgsql(connectionString)
                   .AddInterceptors(sp.GetRequiredService<TenantSchemaInterceptor>());
        });

        // Register Sales & Finance
        services.AddDbContext<SalesDbContext>((sp, options) => {
            options.UseNpgsql(connectionString)
                   .AddInterceptors(sp.GetRequiredService<TenantSchemaInterceptor>());
        });

        // Register Quality Control
        services.AddDbContext<QCDbContext>((sp, options) => {
            options.UseNpgsql(connectionString)
                   .AddInterceptors(sp.GetRequiredService<TenantSchemaInterceptor>());
        });

        return services;
    }
}
