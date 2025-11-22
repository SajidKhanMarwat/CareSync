using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Common;
using CareSync.Shared.Enums;
using System.Text.Json;

namespace CareSync.Pages.Admin;

public class CreateDoctorModel : BasePageModel
{
    private readonly ILogger<CreateDoctorModel> _logger;
    private readonly AdminApiService _adminApiService;

    public CreateDoctorModel(ILogger<CreateDoctorModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromBody] DoctorRegistrationRequest request)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return Unauthorized();

            _logger.LogInformation("Received doctor registration request for: {Email}", request.Email);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.FirstName) || 
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.LicenseNumber) ||
                string.IsNullOrWhiteSpace(request.Specialization))
            {
                return new JsonResult(new { success = false, message = "Please fill in all required fields." });
            }

            // Generate username from email
            var username = request.Email.Split('@')[0];
            
            // Create UserRegisteration_DTO with nested RegisterDoctor_DTO
            var doctorRegistration = new UserRegisteration_DTO
            {
                // User basic info
                FirstName = request.FirstName,
                LastName = request.LastName ?? "",
                MiddleName = "",
                ArabicUserName = request.FirstName, // Default to English name
                ArabicFirstName = request.FirstName, // Default to English name
                ArabicLastName = "",
                Email = request.Email,
                UserName = username,
                PhoneNumber = request.PhoneNumber ?? "",
                Password = "CareSync@123", // Default password - should be changed on first login
                ConfirmPassword = "CareSync@123",
                Gender = Enum.Parse<Gender_Enum>(request.Gender ?? "Male", true),
                DateOfBirth = string.IsNullOrWhiteSpace(request.DateOfBirth) 
                    ? null 
                    : DateTime.Parse(request.DateOfBirth),
                Address = request.Address,
                RoleType = RoleType.Doctor,
                IsActive = true,
                
                // Nested doctor-specific details
                RegisterDoctor = new RegisterDoctor_DTO
                {
                    Specialization = request.Specialization,
                    ArabicSpecialization = request.Specialization,
                    LicenseNumber = request.LicenseNumber,
                    ExperienceYears = request.ExperienceYears,
                    QualificationSummary = request.Qualifications,
                    Certifications = request.Certifications,
                    Department = request.Department,
                    ClinicAddress = request.Address,
                    AvailableDays = request.AvailableDays ?? "Monday, Tuesday, Wednesday, Thursday, Friday",
                    StartTime = request.StartTime ?? "09:00",
                    EndTime = request.EndTime ?? "17:00",
                    AppointmentDuration = request.AppointmentDuration ?? 30,
                    MaxPatientsPerDay = request.MaxPatients ?? 20,
                    CreatedBy = GetCurrentUserId() ?? "admin"
                }
            };

            _logger.LogInformation("Registering new doctor: {Email} with specialization: {Specialization}", 
                request.Email, request.Specialization);

            var result = await _adminApiService.RegisterDoctorAsync<Result<GeneralResponse>>(doctorRegistration);

            if (result?.IsSuccess == true)
            {
                _logger.LogInformation("Doctor {Email} registered successfully", request.Email);
                return new JsonResult(new 
                { 
                    success = true, 
                    message = $"Dr. {request.FirstName} {request.LastName} registered successfully! Default password: CareSync@123"
                });
            }
            else
            {
                var errorMessage = result?.GetError() ?? "Failed to register doctor.";
                _logger.LogWarning("Failed to register doctor: {Error}", errorMessage);
                return new JsonResult(new { success = false, message = errorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering doctor");
            return new JsonResult(new { success = false, message = "An error occurred while registering the doctor." });
        }
    }
    
    private string? GetCurrentUserId()
    {
        return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }
}

/// <summary>
/// Request model for doctor registration from the frontend form
/// </summary>
public class DoctorRegistrationRequest
{
    public string FirstName { get; set; } = "";
    public string? LastName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string LicenseNumber { get; set; } = "";
    public int? ExperienceYears { get; set; }
    public string Specialization { get; set; } = "";
    public string? Department { get; set; }
    public string? Qualifications { get; set; }
    public string? Certifications { get; set; }
    public string? AvailableDays { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public int? AppointmentDuration { get; set; }
    public int? MaxPatients { get; set; }
}
