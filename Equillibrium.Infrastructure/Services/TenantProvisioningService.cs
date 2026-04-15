using Equillibrium.Core.Interfaces;
using Equillibrium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace Equillibrium.Infrastructure.Services;

public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly ApplicationDbContext _context;

    public TenantProvisioningService(ApplicationDbContext context)
    {
        _context = context;
    }
    [SuppressMessage("Security", "EF1002:Runtime SQL injection vulnerability")]
    public async Task ProvisionTenantAsync(string tenantId)
    {
        await _context.Database.OpenConnectionAsync();
        try 
        {
            // 1. Create the Schema
            await _context.Database.ExecuteSqlRawAsync($"CREATE SCHEMA IF NOT EXISTS \"{tenantId}\";");
            
            var createTablesSql = $@"
            CREATE TABLE IF NOT EXISTS ""{tenantId}"".""Contacts"" (
                ""Id"" UUID PRIMARY KEY,
                ""CompanyName"" VARCHAR(200) NOT NULL,
                ""ContactName"" VARCHAR(200),
                ""Email"" VARCHAR(200),
                ""Phone"" VARCHAR(50),
                ""IsDeleted"" BOOLEAN DEFAULT FALSE,
                ""CreatedAt"" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
            );

            CREATE TABLE IF NOT EXISTS ""{tenantId}"".""Materials"" (
                ""Id"" UUID PRIMARY KEY,
                ""Name"" TEXT NOT NULL,
                ""Sku"" TEXT,
                ""Grade"" TEXT,
                ""BaseUnit"" TEXT,
                ""CreatedAt"" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
            );

            CREATE TABLE IF NOT EXISTS ""{tenantId}"".""SupplierMaterials"" (
                ""MaterialId"" UUID NOT NULL REFERENCES ""{tenantId}"".""Materials""(""Id"") ON DELETE CASCADE,
                ""SupplierId"" UUID NOT NULL REFERENCES ""{tenantId}"".""Contacts""(""Id"") ON DELETE CASCADE,
                ""Price"" TEXT,
                ""PurityPercentage"" DECIMAL DEFAULT 0,
                ""SdsUrl"" TEXT,
                ""Note"" TEXT,
                PRIMARY KEY (""MaterialId"", ""SupplierId"")
            );

            CREATE TABLE IF NOT EXISTS ""{tenantId}"".""PurchaseOrders"" (
                ""Id"" UUID PRIMARY KEY,
                ""OrderNumber"" TEXT NOT NULL,
                ""SupplierId"" UUID NOT NULL REFERENCES ""{tenantId}"".""Contacts""(""Id""),
                ""OrderDate"" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
                ""Status"" INTEGER NOT NULL,
                ""TotalAmount"" DECIMAL NOT NULL
            );

            CREATE TABLE IF NOT EXISTS ""{tenantId}"".""PurchaseOrderItems"" (
                ""Id"" UUID PRIMARY KEY,
                ""PurchaseOrderId"" UUID NOT NULL REFERENCES ""{tenantId}"".""PurchaseOrders""(""Id"") ON DELETE CASCADE,
                ""MaterialId"" UUID NOT NULL REFERENCES ""{tenantId}"".""Materials""(""Id""),
                ""Quantity"" DECIMAL NOT NULL,
                ""UnitPrice"" DECIMAL NOT NULL
            );";
            await _context.Database.ExecuteSqlRawAsync(createTablesSql);

            // Step A: Sync Suppliers
            var syncSuppliers = $@"
                INSERT INTO ""{tenantId}"".""Contacts"" (""Id"", ""CompanyName"", ""ContactName"", ""Email"", ""Phone"", ""IsDeleted"", ""CreatedAt"")
                SELECT ""Id"", ""CompanyName"", ""ContactName"", ""Email"", ""Phone"", ""IsDeleted"", NOW() FROM public.""Contacts""
                ON CONFLICT (""Id"") DO NOTHING;";
            await _context.Database.ExecuteSqlRawAsync(syncSuppliers);

            // Step B: Sync Materials
            var syncMaterials = $@"
                INSERT INTO ""{tenantId}"".""Materials"" (""Id"", ""Name"", ""Sku"", ""Grade"", ""BaseUnit"", ""CreatedAt"")
                SELECT ""Id"", ""Name"", ""Sku"", ""Grade"", ""BaseUnit"", NOW() FROM public.""Materials""
                ON CONFLICT (""Id"") DO NOTHING;";
            await _context.Database.ExecuteSqlRawAsync(syncMaterials);

            // Step C: Sync Links (Safe version with plural names)
            var syncLinks = $@"
                INSERT INTO ""{tenantId}"".""SupplierMaterials"" (""MaterialId"", ""SupplierId"", ""Price"", ""PurityPercentage"", ""SdsUrl"", ""Note"")
                SELECT ""MaterialId"", ""SupplierId"", ""Price"", ""PurityPercentage"", ""SdsUrl"", ""Note"" 
                FROM public.""SupplierMaterials""
                WHERE EXISTS (SELECT 1 FROM ""{tenantId}"".""Materials"" m WHERE m.""Id"" = ""MaterialId"")
                AND EXISTS (SELECT 1 FROM ""{tenantId}"".""Contacts"" c WHERE c.""Id"" = ""SupplierId"")
                ON CONFLICT (""MaterialId"", ""SupplierId"") DO NOTHING;";
            await _context.Database.ExecuteSqlRawAsync(syncLinks);

        }
        finally 
        {
            await _context.Database.CloseConnectionAsync();
        }
    }

}