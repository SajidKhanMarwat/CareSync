using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Common;
using CareSync.Shared.Enums;
using CareSync.Shared.Enums.Patient;

namespace CareSync.Pages.Admin.Patients;

public class CreatePatientModel : BasePageModel
{
    private readonly ILogger<CreatePatientModel> _logger;
    private readonly AdminApiService _adminApiService;

    public CreatePatientModel(ILogger<CreatePatientModel> logger, AdminApiService adminApiService)
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

    public async Task<IActionResult> OnPostAsync([FromBody] PatientRegistrationRequest request)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return Unauthorized();

            _logger.LogInformation("Received patient registration request for: {Email}", request.Email);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.FirstName) || 
                string.IsNullOrWhiteSpace(request.Email))
            {
                return new JsonResult(new { success = false, message = "Please fill in all required fields." });
            }

            // Generate username from email
            var username = request.Email.Split('@')[0];
            
            // Create UserRegisteration_DTO with nested RegisterPatient_DTO
            var patientRegistration = new UserRegisteration_DTO
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
                RoleType = RoleType.Patient,
                IsActive = true,
                
                // Nested patient-specific details
                RegisterPatient = new RegisterPatient_DTO
                {
                    BloodGroup = request.BloodGroup,
                    MaritalStatus = string.IsNullOrWhiteSpace(request.MaritalStatus)
                        ? MaritalStatusEnum.Single
                        : Enum.Parse<MaritalStatusEnum>(request.MaritalStatus, true),
                    Occupation = request.Occupation,
                    EmergencyContactName = request.EmergencyContactName,
                    EmergencyContactNumber = request.EmergencyContactNumber,
                    RelationshipToEmergency = request.RelationshipToEmergency,
                    CreatedBy = GetCurrentUserId() ?? "admin"
                }
            };

            _logger.LogInformation("Registering new patient: {Email}", request.Email);

            var result = await _adminApiService.RegisterPatientAsync<Result<GeneralResponse>>(patientRegistration);

            if (result?.IsSuccess == true)
            {
                _logger.LogInformation("Patient {Email} registered successfully", request.Email);
                return new JsonResult(new 
                { 
                    success = true, 
                    message = $"{request.FirstName} {request.LastName} registered successfully! Default password: CareSync@123"
                });
            }
            else
            {
                var errorMessage = result?.GetError() ?? "Failed to register patient.";
                _logger.LogWarning("Failed to register patient: {Error}", errorMessage);
                return new JsonResult(new { success = false, message = errorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering patient");
            return new JsonResult(new { success = false, message = "An error occurred while registering the patient." });
        }
    }
    
    private string? GetCurrentUserId()
    {
        return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }
}

/// <summary>
/// Request model for patient registration from the frontend form
/// </summary>
public class PatientRegistrationRequest
{
    public string FirstName { get; set; } = "";
    public string? LastName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? BloodGroup { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Occupation { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactNumber { get; set; }
    public string? RelationshipToEmergency { get; set; }
}
