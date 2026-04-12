using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Security.Tenant;

public static class TenantServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddTenantContext()
        {
            services.AddScoped<ITenantContext, TenantContext>();
            return services;
        }
    }

    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseTenantContext()
        {
            app.UseMiddleware<TenantMiddleware>();
            return app;
        }
    }
}
