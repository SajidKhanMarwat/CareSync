using CareSync.DataLayer.Entities;
using CareSync.DataLayer.DataEnums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CareSync.ApplicationLayer.Services.SeedData;

public class RoleSeedService
{
    private readonly RoleManager<T_Roles> _roleManager;
    private readonly ILogger<RoleSeedService> _logger;

    public RoleSeedService(RoleManager<T_Roles> roleManager, ILogger<RoleSeedService> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedRolesAsync()
    {
        _logger.LogInformation("Starting role seeding process...");

        var roles = new List<(string Name, RoleType RoleType, string Description)>
        {
            ("Admin", RoleType.Admin, "System Administrator with full access to all features"),
            ("Patient", RoleType.Patient, "Patients who receive medical care and services"),
            ("Doctor", RoleType.Doctor, "Medical doctors who provide patient care"),
            ("DoctorAssistant", RoleType.DoctorAssistant, "Assistant doctors who support medical care"),
            ("LabAssistant", RoleType.LabAssistant, "Laboratory assistants who help with lab operations"),
            ("Lab", RoleType.Lab, "Laboratory technicians who perform medical tests")
        };

        foreach (var (name, roleType, description) in roles)
        {
            await CreateRoleIfNotExistsAsync(name, roleType, description);
        }

        _logger.LogInformation("Role seeding process completed.");
    }

    private async Task CreateRoleIfNotExistsAsync(string roleName, RoleType roleType, string description)
    {
        try
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new T_Roles
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    RoleName = roleName,
                    RoleArabicName = GetArabicRoleName(roleName),
                    RoleType = roleType,
                    Description = description,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Role '{RoleName}' created successfully.", roleName);
                }
                else
                {
                    _logger.LogError("Failed to create role '{RoleName}'. Errors: {Errors}", 
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogInformation("Role '{RoleName}' already exists.", roleName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating role '{RoleName}'", roleName);
        }
    }

    private static string GetArabicRoleName(string roleName)
    {
        return roleName switch
        {
            "Admin" => "مدير النظام",
            "Patient" => "مريض",
            "Doctor" => "طبيب",
            "DoctorAssistant" => "مساعد طبيب",
            "LabAssistant" => "مساعد مختبر",
            "Lab" => "فني مختبر",
            _ => roleName
        };
    }
}
