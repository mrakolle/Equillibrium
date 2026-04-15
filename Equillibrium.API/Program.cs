using Equillibrium.Core.Interfaces;
using Equillibrium.Infrastructure.Data;
using Equillibrium.Infrastructure.Data.Interceptors;
using Equillibrium.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CORE SERVICES ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- 2. SWAGGER CONFIG (Global Tenant ID) ---
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Equillibrium ERP", Version = "v1" });

    options.AddSecurityDefinition("TenantId", new OpenApiSecurityScheme
    {
        Name = "X-Tenant-Id",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Enter Tenant ID (UUID). This will appear in the Swagger Curl."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "TenantId" }
            },
            Array.Empty<string>()
        }
    });
});

// --- 3. INFRASTRUCTURE SERVICES ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
builder.Services.AddScoped<TenantSchemaInterceptor>();

// --- 4. DATABASE CONFIG (Check ConnectionString Name) ---
// IMPORTANT: Ensure "DefaultConnection" matches your appsettings.json key!
var connectionString = builder.Configuration.GetConnectionString("MasterDb");

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
app.UseAuthorization();
app.MapControllers();
app.Run();
