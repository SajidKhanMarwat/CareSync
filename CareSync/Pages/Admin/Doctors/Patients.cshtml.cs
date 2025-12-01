using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Admin.Doctors;

[Authorize(Roles = "Admin")]
public class DoctorPatientsModel : PageModel
{
    private readonly AdminApiService _adminApiService;
    private readonly ILogger<DoctorPatientsModel> _logger;

    public DoctorPatientsModel(AdminApiService adminApiService, ILogger<DoctorPatientsModel> logger)
    {
        _adminApiService = adminApiService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = string.Empty;

    public List<PatientList_DTO> Patients { get; set; } = new();
    public string? DoctorName { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            TempData["ErrorMessage"] = "Doctor ID is required.";
            return RedirectToPage("/Admin/Doctors/DoctorsList");
        }

        try
        {
            // Get doctor name first
            var doctorResult = await _adminApiService.GetDoctorProfileAsync<Result<ApplicationLayer.Contracts.DoctorsDTOs.DoctorProfile_DTO>>(Id);
            if (doctorResult?.IsSuccess == true && doctorResult.Data != null)
            {
                DoctorName = doctorResult.Data.FullName;
            }

            // Get patients
            var result = await _adminApiService.GetDoctorPatientsAsync<Result<List<PatientList_DTO>>>(Id);
            if (result?.IsSuccess == true && result.Data != null)
            {
                Patients = result.Data;
            }
            else
            {
                // Even if no patients, we can still show the page
                Patients = new List<PatientList_DTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor patients for ID: {Id}", Id);
            TempData["ErrorMessage"] = "An error occurred while loading the patient list.";
            return RedirectToPage("/Admin/Doctors/DoctorsList");
        }

        return Page();
    }
}
