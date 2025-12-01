using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using Microsoft.Extensions.Logging;

namespace CareSync.Pages.Admin.Patients;

public class EditPatientModel : BasePageModel
{
    private readonly ILogger<EditPatientModel> _logger;
    private readonly AdminApiService _adminApiService;

    public EditPatientModel(ILogger<EditPatientModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    [BindProperty]
    public PatientList_DTO? Patient { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Patient ID is required";
                return RedirectToPage("/Admin/Patients/PatientsList");
            }

            _logger.LogInformation("Loading patient for editing - ID: {PatientId}", id.Value);

            // Load all patients and find the specific one
            var patientsResult = await _adminApiService.GetAllPatientsAsync<Result<List<PatientList_DTO>>>(null, null);
            
            if (patientsResult?.IsSuccess == true && patientsResult.Data != null)
            {
                Patient = patientsResult.Data.FirstOrDefault(p => p.PatientID == id.Value);
                
                if (Patient == null)
                {
                    TempData["ErrorMessage"] = "Patient not found";
                    _logger.LogWarning("Patient with ID {PatientId} not found", id.Value);
                    return RedirectToPage("/Admin/Patients/PatientsList");
                }
                
                _logger.LogInformation("Successfully loaded patient for editing: {PatientName}", Patient.FullName);
            }
            else
            {
                TempData["ErrorMessage"] = patientsResult?.GetError() ?? "Failed to load patient data";
                _logger.LogError("Failed to load patients for editing");
                return RedirectToPage("/Admin/Patients/PatientsList");
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient for editing");
            TempData["ErrorMessage"] = "An error occurred while loading the patient";
            return RedirectToPage("/Admin/Patients/PatientsList");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the validation errors";
                return Page();
            }

            if (Patient == null)
            {
                TempData["ErrorMessage"] = "Patient data is missing";
                return Page();
            }

            _logger.LogInformation("Updating patient: {PatientId}", Patient.PatientID);

            // Create update DTO
            var updateDto = new UserPatientProfileUpdate_DTO
            {
                UserId = Patient.UserID,
                FirstName = Patient.FirstName,
                LastName = Patient.LastName,
                Email = Patient.Email,
                PhoneNumber = Patient.PhoneNumber,
                DateOfBirth = Patient.DateOfBirth,
                Gender = Patient.Gender,
                Address = Patient.Address,
                BloodGroup = Patient.BloodGroup,
                MaritalStatus = Patient.MaritalStatus.ToString(),
                Occupation = Patient.Occupation,
                EmergencyContactName = Patient.EmergencyContactName,
                EmergencyContactNumber = Patient.EmergencyContactNumber,
                RelationshipToEmergency = Patient.RelationshipToEmergency
            };

            // Call the update API endpoint
            var result = await _adminApiService.UpdatePatientAsync<Result<GeneralResponse>>(updateDto);
            
            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Patient information updated successfully";
                _logger.LogInformation("Successfully updated patient: {PatientId}", Patient.PatientID);
            }
            else
            {
                TempData["ErrorMessage"] = result?.Data?.Message ?? "Failed to update patient information";
                _logger.LogError("Failed to update patient: {PatientId}", Patient.PatientID);
                return Page();
            }
            
            return RedirectToPage("/Admin/Patients/Profile", new { id = Patient.PatientID });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating patient");
            TempData["ErrorMessage"] = "An error occurred while updating the patient";
            return Page();
        }
    }
}
