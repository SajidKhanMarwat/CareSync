using CareSync.ApplicationLayer.Services.SeedData;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CareSync.ApplicationLayer.Extensions;

public static class SeedDataExtensions
{
    /// <summary>
    /// Seeds the database with initial data including roles
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static async Task<IApplicationBuilder> SeedDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DatabaseSeeder");

        try
        {
            logger.LogInformation("Starting database seeding...");

            // Seed roles
            var roleSeedService = services.GetRequiredService<RoleSeedService>();
            await roleSeedService.SeedRolesAsync();

            // Seed admin user
            var adminSeedService = services.GetRequiredService<AdminSeedService>();
            await adminSeedService.SeedAdminUserAsync();

            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }

        return app;
    }

    /// <summary>
    /// Registers seed data services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSeedDataServices(this IServiceCollection services)
    {
        services.AddScoped<RoleSeedService>();
        services.AddScoped<AdminSeedService>();
        return services;
    }
}
