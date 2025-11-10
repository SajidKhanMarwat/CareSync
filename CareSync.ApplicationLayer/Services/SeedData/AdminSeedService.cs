using CareSync.DataLayer.Entities;
using CareSync.DataLayer.DataEnums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CareSync.ApplicationLayer.Services.SeedData;

public class AdminSeedService
{
    private readonly UserManager<T_Users> _userManager;
    private readonly RoleManager<T_Roles> _roleManager;
    private readonly ILogger<AdminSeedService> _logger;

    public AdminSeedService(
        UserManager<T_Users> userManager, 
        RoleManager<T_Roles> roleManager,
        ILogger<AdminSeedService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAdminUserAsync()
    {
        _logger.LogInformation("Starting admin user seeding process...");

        const string adminEmail = "admin@caresync.com";
        const string adminPassword = "Admin@123456";

        try
        {
            // Check if admin user already exists
            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null)
            {
                _logger.LogInformation("Admin user already exists.");
                return;
            }

            // Get Admin role
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                _logger.LogError("Admin role not found. Please ensure roles are seeded first.");
                return;
            }

            // Create admin user
            var adminUser = new T_Users
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                ArabicUserName = "مدير النظام",
                LoginID = 1,
                RoleID = adminRole.Id,
                RoleType = RoleType.Admin,
                Gender = Gender.Other,
                IsActive = true,
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                // Assign Admin role
                var roleResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
                
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("Admin user created successfully with email: {Email}", adminEmail);
                    _logger.LogInformation("Default admin password: {Password}", adminPassword);
                    _logger.LogWarning("Please change the default admin password after first login!");
                }
                else
                {
                    _logger.LogError("Failed to assign Admin role to user. Errors: {Errors}", 
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogError("Failed to create admin user. Errors: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating admin user");
        }
    }
}
