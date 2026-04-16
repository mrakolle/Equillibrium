using Equillibrium.Core.Interfaces;

namespace Equillibrium.API.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        // 1. Extract the Tenant ID from the Header
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantId))
        {
            // 2. Pass it to the scoped TenantService (logic we implemented in ITenantService)
            // The Interceptor will call GetTenantSchema() later using this context.
            // For now, we simply ensure the context is aware of the request.
        }

        // 3. Move to the next piece of the pipeline
        await _next(context);
    }
}
