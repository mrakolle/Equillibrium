using System.Data.Common;
using Equillibrium.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Equillibrium.Infrastructure.Data.Interceptors;

public class TenantSchemaInterceptor : DbConnectionInterceptor
{
    private readonly ITenantService _tenantService;

    public TenantSchemaInterceptor(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    // This runs AFTER the connection is physically open and ready for commands
    public override void ConnectionOpened(
        DbConnection connection, 
        ConnectionEndEventData eventData)
    {
        var tenantId = _tenantService.GetTenantId();
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            using var command = connection.CreateCommand();
            // Quotes handle the hyphens in the UUID schema name
            command.CommandText = $"SET search_path TO \"{tenantId}\", public;";
            command.ExecuteNonQuery();
        }

        base.ConnectionOpened(connection, eventData);
    }

    // Async version for async queries
    public override async Task ConnectionOpenedAsync(
        DbConnection connection, 
        ConnectionEndEventData eventData, 
        CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantService.GetTenantId();
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"SET search_path TO \"{tenantId}\", public;";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
}