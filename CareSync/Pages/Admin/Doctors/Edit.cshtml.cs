using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Admin.Doctors;

[Authorize(Roles = "Admin")]
public class EditDoctorModel : PageModel
{
    private readonly AdminApiService _adminApiService;
    private readonly ILogger<EditDoctorModel> _logger;

    public EditDoctorModel(AdminApiService adminApiService, ILogger<EditDoctorModel> logger)
    {
        _adminApiService = adminApiService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = string.Empty;

    [BindProperty]
    public UpdateDoctor_DTO Doctor { get; set; } = new();

    public string? ProfileImage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            TempData["ErrorMessage"] = "Doctor ID is required.";
            return RedirectToPage("/Admin/Doctors/DoctorsList");
        }

        try
        {
            var result = await _adminApiService.GetDoctorProfileAsync<Result<DoctorProfile_DTO>>(Id);
            if (result?.IsSuccess == true && result.Data != null)
            {
                var profile = result.Data;
                Doctor = new UpdateDoctor_DTO
                {
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Email = profile.Email,
                    PhoneNumber = profile.PhoneNumber,
                    DateOfBirth = profile.DateOfBirth,
                    Address = profile.Address,
                    Specialization = profile.Specialization,
                    LicenseNumber = profile.LicenseNumber,
                    ExperienceYears = profile.ExperienceYears,
                    HospitalAffiliation = profile.HospitalAffiliation,
                    Qualifications = profile.Qualifications,
                    AvailableDays = profile.AvailableDays,
                    StartTime = profile.StartTime,
                    EndTime = profile.EndTime,
                    ConsultationFee = profile.ConsultationFee
                };
                ProfileImage = profile.ProfileImage;
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to load doctor profile.";
                return RedirectToPage("/Admin/Doctors/DoctorsList");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor profile for editing: {Id}", Id);
            TempData["ErrorMessage"] = "An error occurred while loading the doctor profile.";
            return RedirectToPage("/Admin/Doctors/DoctorsList");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var result = await _adminApiService.UpdateDoctorAsync<Result<GeneralResponse>>(Id, Doctor);
            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Doctor information updated successfully.";
                return RedirectToPage("/Admin/Doctors/Profile", new { id = Id });
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to update doctor information.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor: {Id}", Id);
            TempData["ErrorMessage"] = "An error occurred while updating the doctor information.";
            return Page();
        }
    }
}
