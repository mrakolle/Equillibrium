using Equillibrium.Core.Interfaces;
using Equillibrium.Infrastructure.Data;
using Equillibrium.Infrastructure.Data.Persistence; // Ensure this matches your Interceptor path
using Equillibrium.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Equillibrium.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CORE SERVICES ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- 2. DATABASE CONFIG (Move this UP) ---
var connectionString = builder.Configuration.GetConnectionString("MasterDb");

// --- 3. SWAGGER CONFIG ---
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Equillibrium ERP", Version = "v1" });
    options.AddSecurityDefinition("TenantId", new OpenApiSecurityScheme
    {
        Name = "X-Tenant-Id",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Enter Tenant ID (UUID)."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "TenantId" }
            }, Array.Empty<string>()
        }
    });
});

// --- 4. INFRASTRUCTURE & MODULES ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();

// Register the Interceptor
builder.Services.AddScoped<TenantSchemaInterceptor>();

// IMPORTANT: Register all modular "Apps" (Sales, Manufacturing, QC)
builder.Services.AddEcosystemModules(connectionString!);

// Register the main "OS" Context
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<TenantSchemaInterceptor>();
    options.UseNpgsql(connectionString)
           .AddInterceptors(interceptor);
});

var app = builder.Build();

// --- 5. PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add the Middleware we discussed earlier to resolve tenant context
app.UseMiddleware<TenantMiddleware>(); 

app.UseAuthorization();
app.MapControllers();
app.Run();
