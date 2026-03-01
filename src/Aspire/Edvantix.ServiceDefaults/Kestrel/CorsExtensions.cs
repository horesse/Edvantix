using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Edvantix.ServiceDefaults.Kestrel;

public static class CorsExtensions
{
    private const string AllowAllCorsPolicy = "AllowAll";
    private const string AllowSpecificCorsPolicy = "AllowSpecific";

    public static void AddDefaultCors(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    AllowAllCorsPolicy,
                    policyBuilder =>
                    {
                        policyBuilder
                            .SetIsOriginAllowed(origin => new Uri(origin).Host == Network.Localhost)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                );
            });
        }
        else
        {
            services.Configure<CorsSettings>(CorsSettings.ConfigurationSection);

            services.AddCors(options =>
            {
                options.AddPolicy(
                    AllowSpecificCorsPolicy,
                    policyBuilder =>
                    {
                        var serviceProvider = services.BuildServiceProvider();
                        var corsOptions = serviceProvider.GetRequiredService<CorsSettings>();

                        policyBuilder
                            .WithOrigins([.. corsOptions.Origins])
                            .WithHeaders([.. corsOptions.Headers])
                            .WithMethods([.. corsOptions.Methods]);

                        if (corsOptions.MaxAge is not null)
                        {
                            policyBuilder.SetPreflightMaxAge(
                                TimeSpan.FromSeconds(corsOptions.MaxAge.Value)
                            );
                        }

                        if (corsOptions.AllowCredentials)
                        {
                            policyBuilder.AllowCredentials();
                        }
                    }
                );
            });
        }
    }

    public static void UseDefaultCors(this WebApplication app)
    {
        var policyName = app.Environment.IsDevelopment()
            ? AllowAllCorsPolicy
            : AllowSpecificCorsPolicy;
        app.UseCors(policyName);
    }
}
