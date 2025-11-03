using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CareSync.ApplicationLayer;

public static class ApplicationLayerDependencies
{
    public static IServiceCollection ApplicationLayerServices(this IServiceCollection services)
    {
        #region Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File($"Logs/logs{DateTime.Today.Date}.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger();

        services.AddSerilog();
        #endregion
        return services;
    }

    public static IApplicationBuilder ApplicationLayerDI(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}