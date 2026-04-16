using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Equillibrium.Core.Interfaces;

namespace Equillibrium.Infrastructure.Data.Persistence;

public class TenantSchemaInterceptor : DbConnectionInterceptor
{
    private readonly ITenantService _tenantService;

    public TenantSchemaInterceptor(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public override InterceptionResult ConnectionOpening(
        DbConnection connection, 
        ConnectionEventData eventData, 
        InterceptionResult result)
    {
        var tenantSchema = _tenantService.GetTenantSchema(); // e.g., "tenant_a"
        
        using var command = connection.CreateCommand();
        // This is the magic line for Postgres Multi-Schema
        command.CommandText = $"SET search_path TO {tenantSchema}, public;";
        command.ExecuteNonQuery();

        return result;
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection, 
        ConnectionEventData eventData, 
        InterceptionResult result, 
        CancellationToken cancellationToken = default)
    {
        var tenantSchema = _tenantService.GetTenantSchema();
        
        await using var command = connection.CreateCommand();
        command.CommandText = $"SET search_path TO {tenantSchema}, public;";
        await command.ExecuteNonQueryAsync(cancellationToken);

        return result;
    }
}
