using CareSync.ApplicationLayer.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CareSync.ApplicationLayer;

public static class ApplicationLayerDependencies
{
    public static IServiceCollection ApplicationLayerServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationLayerDependencies).Assembly);
        services.AddSeedDataServices();
        
        //#region Serilog
        //Log.Logger = new LoggerConfiguration()
        //    .MinimumLevel.Information()
        //    .WriteTo.File($"Logs/logs{DateTime.Today.Date}.txt", rollingInterval: RollingInterval.Day)
        //    .WriteTo.Console()
        //    .CreateLogger();

        //services.AddSerilog();
        //#endregion

        //#region Services
        //services.AddScoped<IUserService, UserService>();
        //#endregion
        return services;
    }

    public static IApplicationBuilder ApplicationLayerDI(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}