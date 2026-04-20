using Equillibrium.Core.Interfaces;
using Equillibrium.Infrastructure.Data;
using Equillibrium.Infrastructure.Data.Persistence;
using Equillibrium.Infrastructure.Services;
using Equillibrium.API.Middleware;
using Equillibrium.Manufacturing.Controllers;
//using Equillibrium.Manufacturing.Controllers;
using Equillibrium.Sales.Controllers;
using Equillibrium.QC.Controllers;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DATABASE CONFIG ---
var connectionString = builder.Configuration.GetConnectionString("MasterDb");

// --- 2. CORE SERVICES & CONTROLLER DISCOVERY ---
builder.Services.AddControllers()
    .AddApplicationPart(typeof(ManufacturingController).Assembly)
    .AddApplicationPart(typeof(SalesController).Assembly)
    .AddApplicationPart(typeof(QCController).Assembly);


builder.Services.AddEndpointsApiExplorer();

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

// --- 4. INFRASTRUCTURE & MODULAR APPS ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
builder.Services.AddScoped<TenantSchemaInterceptor>();

// This wires up Sales, Mfg, and QC contexts
builder.Services.AddEcosystemModules(connectionString!);

// Main OS Context
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

// The "Traffic Cop" that sets the Tenant ID for the Interceptor
app.UseMiddleware<TenantMiddleware>(); 

app.UseAuthorization();
app.MapControllers();
app.Run();
