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

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection, 
        ConnectionEventData eventData, 
        InterceptionResult result, 
        CancellationToken cancellationToken = default)
    {
        // If the connection is already open (like during our manual provisioning loop), 
        // do NOT change the search_path.
        if (connection.State == System.Data.ConnectionState.Open)
        {
            return result;
        }

        var schema = _tenantService.GetTenantSchema();

        // Only intercept if we have a valid tenant and it's not public
        if (!string.IsNullOrEmpty(schema) && schema != "public")
        {
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = $"SET search_path TO \"{schema}\", public;";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        return result;
    }

    /*public override InterceptionResult ConnectionOpening(
        DbConnection connection, 
        ConnectionEventData eventData, 
        InterceptionResult result)
    {
        var schema = _tenantService.GetTenantSchema();

        // Skip if schema is public/null (prevents errors during migrations)
        if (!string.IsNullOrEmpty(schema) && schema != "public")
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"SET search_path TO \"{schema}\", public;";
            command.ExecuteNonQuery();
        }

        return result;
    }*/

    /*public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
    DbConnection connection, 
    ConnectionEventData eventData, 
    InterceptionResult result, 
    CancellationToken cancellationToken = default)
    {
        // 1. Check if the Provisioning Service already set a search_path
        // We do this by checking the connection state or a custom state flag
        if (connection.State == System.Data.ConnectionState.Open)
        {
            return result; // Skip if already open and configured
        }

        var schema = _tenantService.GetTenantSchema();

        // 2. Only override if we have a valid tenant and it's not a provisioning task
        if (!string.IsNullOrEmpty(schema) && schema != "public")
        {
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = $"SET search_path TO \"{schema}\", public;";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        return result;
    }*/

}
