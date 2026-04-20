using Equillibrium.Core.Interfaces;
using Equillibrium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;
using Equillibrium.Manufacturing.Infrastructure.Data;
using Equillibrium.Sales.Infrastructure.Data;
using Equillibrium.QC.Infrastructure.Data;
using System.Data.Common;

namespace Equillibrium.Infrastructure.Services;

public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly ApplicationDbContext _context;
    private readonly ManufacturingDbContext _mfgDb;
    private readonly SalesDbContext _salesDb;
    private readonly QCDbContext _qcDb;

    public TenantProvisioningService(
        ApplicationDbContext context,
        ManufacturingDbContext mfgDb, 
        SalesDbContext salesDb, 
        QCDbContext qcDb)
    {
        _context = context;
        _mfgDb = mfgDb;
        _salesDb = salesDb;
        _qcDb = qcDb;
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "EF1002")]
    public async Task ProvisionTenantAsync(string tenantId)
    {
        var safeId = tenantId.Replace("-", "_");
        var schemaName = $"tenant_{safeId}";

        // METHOD 1: Create the physical "House" (Schema & Tables)
        await CreateTenantInfrastructureAsync(schemaName);

        // METHOD 2: Fill the house with "Furniture" (Global Data)
        // This can now be called anytime (onboarding, system updates, etc.)
       await PopulateTenantDataAsync(schemaName); 
    }

    public async Task CreateTenantInfrastructureAsync(string schemaName)
    {
        var connString = _mfgDb.Database.GetDbConnection().ConnectionString;
        using var conn = new Npgsql.NpgsqlConnection(connString);
        await conn.OpenAsync();

        // 1. Create Schema
        using (var cmd = new Npgsql.NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\";", conn))
            await cmd.ExecuteNonQueryAsync();

        // 2. Build Tables using the "Naked" scripts we refined
        // (Ensure your ExecuteQualifiedScript is called here for all 3 modules)
        await ExecuteQualifiedScript(conn, _mfgDb, schemaName);
        await ExecuteQualifiedScript(conn, _salesDb, schemaName);
        await ExecuteQualifiedScript(conn, _qcDb, schemaName);
        
        // Connection closes here, forcing Postgres to commit the new tables.
    }

    public async Task PopulateTenantDataAsync(string schemaName)
    {
        var connString = _mfgDb.Database.GetDbConnection().ConnectionString;
        using var conn = new Npgsql.NpgsqlConnection(connString);
        await conn.OpenAsync();

        // 1. Fetch Global Data into Memory
        var contacts = await _context.Contacts.AsNoTracking().IgnoreQueryFilters().ToListAsync();
        var materials = await _context.Materials.AsNoTracking().IgnoreQueryFilters().ToListAsync();

        // 2. Sync Contacts (Singular: "Contact")
        foreach (var c in contacts)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                INSERT INTO ""{schemaName}"".""Contact"" 
                (""Id"", ""CompanyName"", ""ContactName"", ""Email"", ""Phone"", ""IsDeleted"", ""CreatedAt"") 
                VALUES (@id, @name, @contact, @email, @phone, @isDeleted, @createdAt)
                ON CONFLICT (""Id"") DO NOTHING;";
            
            cmd.Parameters.AddWithValue("id", c.Id);
            cmd.Parameters.AddWithValue("name", c.CompanyName ?? string.Empty);
            cmd.Parameters.AddWithValue("contact", (object?)c.ContactName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("email", (object?)c.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("phone", (object?)c.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("isDeleted", false);
            cmd.Parameters.AddWithValue("createdAt", DateTime.UtcNow);
            
            await cmd.ExecuteNonQueryAsync();
        }

        // 3. Sync Materials (Verified Structure: Added CurrentStock and MinimumStockLevel)
        foreach (var m in materials)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                INSERT INTO ""{schemaName}"".""Material"" 
                (""Id"", ""Name"", ""Sku"", ""CasNumber"", ""Grade"", ""BaseUnit"", ""CurrentStock"", ""MinimumStockLevel"", ""CreatedAt"") 
                VALUES (@id, @name, @sku, @cas, @grade, @unit, @stock, @minStock, @createdAt)
                ON CONFLICT (""Id"") DO NOTHING;";
            
            cmd.Parameters.AddWithValue("id", m.Id);
            cmd.Parameters.AddWithValue("name", m.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("sku", (object?)m.Sku ?? DBNull.Value);
            cmd.Parameters.AddWithValue("cas", (object?)m.CasNumber ?? DBNull.Value); // New field verified from SQL
            cmd.Parameters.AddWithValue("grade", (object?)m.Grade ?? DBNull.Value);
            cmd.Parameters.AddWithValue("unit", (object?)m.BaseUnit ?? DBNull.Value);
            
            // Satisfying NOT NULL constraints verified from SQL
            cmd.Parameters.AddWithValue("stock", 0m); 
            cmd.Parameters.AddWithValue("minStock", 0m);
            cmd.Parameters.AddWithValue("createdAt", DateTime.UtcNow);
            
            await cmd.ExecuteNonQueryAsync();
        }
    }

    /*public async Task ProvisionTenantAsync(string tenantId)
    {
        var safeId = tenantId.Replace("-", "_");
        var schemaName = $"tenant_{safeId}";
        var connString = _mfgDb.Database.GetDbConnection().ConnectionString;

        // --- PHASE 1: BUILD THE ROOM ---
        using (var conn1 = new Npgsql.NpgsqlConnection(connString))
        {
            await conn1.OpenAsync();
            // 1. Create Schema
            using (var cmd = new Npgsql.NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\";", conn1))
                await cmd.ExecuteNonQueryAsync();

            // 2. Create Tables
            await ExecuteQualifiedScript(conn1, _mfgDb, schemaName);
            await ExecuteQualifiedScript(conn1, _salesDb, schemaName);
            await ExecuteQualifiedScript(conn1, _qcDb, schemaName);
            
            await conn1.CloseAsync(); // PHYSICAL BREAK: Forces Postgres to commit the new tables
        }

        // --- PHASE 2: FILL THE ROOM ---
        // We open a COMPLETELY NEW connection. This one is guaranteed to see the new tables.
        using (var conn2 = new Npgsql.NpgsqlConnection(connString))
        {
            await conn2.OpenAsync();
            var syncSql = $@"
                INSERT INTO ""{schemaName}"".""Contacts"" (""Id"", ""CompanyName"", ""ContactName"")
                SELECT ""Id"", ""CompanyName"", ""ContactName"" FROM public.""Contacts""
                ON CONFLICT DO NOTHING;";

            using (var cmd = new Npgsql.NpgsqlCommand(syncSql, conn2))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }*/

    private async Task ExecuteQualifiedScript(Npgsql.NpgsqlConnection conn, DbContext context, string schema)
    {
        var script = context.Database.GenerateCreateScript();
        
        // 1. Strip the hardcoded schema prefixes
        var nakedScript = script
            .Replace("public.", "").Replace("\"public\".", "")
            .Replace("tenants.", "").Replace("\"tenants\".", "")
            .Replace("Tenants.", "").Replace("\"Tenants\".", "");

        using (var cmd = new Npgsql.NpgsqlCommand())
        {
            cmd.Connection = conn;
            // 2. THE ISOLATION TANK: 
            // We set the search_path to ONLY the new schema. 
            // By NOT including 'public', Postgres cannot see the existing PKs or tables,
            // so it is forced to create them fresh in your new schema.
            cmd.CommandText = $"SET search_path TO \"{schema}\"; {nakedScript}";
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task ExecuteScriptInTransaction(DbContext context, string schema, DbConnection conn, DbTransaction trans)
    {
        var script = context.Database.GenerateCreateScript()
            .Replace("public.", "")
            .Replace("\"public\".", "");

        using var cmd = conn.CreateCommand();
        cmd.Transaction = trans;
        cmd.CommandText = $"SET search_path TO \"{schema}\"; {script}";
        await cmd.ExecuteNonQueryAsync();
    }


    private async Task ExecuteScriptInSchema(DbContext context, string schema)
    {
        var rawScript = context.Database.GenerateCreateScript();

        await context.Database.OpenConnectionAsync();
        try {
            using var cmd = context.Database.GetDbConnection().CreateCommand();
            
            // 1. ISOLATE THE PATH: We set the search_path to ONLY the new schema.
            // We remove 'public' from the path here so Postgres CANNOT see
            // any existing tables, preventing the 42P07 error entirely.
            cmd.CommandText = $"SET search_path TO \"{schema}\";";
            await cmd.ExecuteNonQueryAsync();

            // 2. RUN THE SCRIPT: Since the path is isolated, 'CREATE TABLE "BatchRecords"'
            // will now have no choice but to land in the new, empty tenant schema.
            await context.Database.ExecuteSqlRawAsync(rawScript);
            
            Console.WriteLine($"SUCCESS: Tables created in schema {schema}");
        } 
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in ExecuteScriptInSchema: {ex.Message}");
            throw;
        }
        finally {
            await context.Database.CloseConnectionAsync();
        }
    }

   /* private async Task ApplyModularMigration(DbContext context, string schema)
    {
        // 1. Get the blueprint
        var rawScript = context.Database.GenerateCreateScript();

        // 2. STRIP ALL PRE-DEFINED SCHEMAS
        // This forces the tables to land in whatever schema is at the top of the search_path
        var cleanScript = rawScript
            .Replace("public.", "")
            .Replace("\"public\".", "")
            .Replace("tenants.", "")
            .Replace("\"tenants\".", "");

        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open) 
            await connection.OpenAsync();

        try 
        {
            using var cmd = connection.CreateCommand();
            // FORCE the destination to be ONLY the new tenant schema
            cmd.CommandText = $"SET search_path TO \"{schema}\";";
            await cmd.ExecuteNonQueryAsync();

            // Run the "clean" script that has no hardcoded schema addresses
            await context.Database.ExecuteSqlRawAsync(cleanScript);
        } 
        finally 
        {
            // Connection management
        }
    }*/

    /*
    // Helper method to force EF migrations into a specific schema
    private async Task MigrateToSchemaAsync(DbContext context, string schema)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

        // Force the session path for the raw SQL migration runner
        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SET search_path TO \"{schema}\";";
        await cmd.ExecuteNonQueryAsync();

        // Now run MigrateAsync while the connection is pinned to this schema
        await context.Database.MigrateAsync();
    }
    */
}