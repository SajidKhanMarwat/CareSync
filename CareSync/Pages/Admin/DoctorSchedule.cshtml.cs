using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DoctorScheduleModel : PageModel
{
    private readonly AdminApiService _adminApiService;
    private readonly ILogger<DoctorScheduleModel> _logger;

    public DoctorScheduleModel(AdminApiService adminApiService, ILogger<DoctorScheduleModel> logger)
    {
        _adminApiService = adminApiService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = string.Empty;

    public DoctorSchedule_DTO? Schedule { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            TempData["ErrorMessage"] = "Doctor ID is required.";
            return RedirectToPage("/Admin/Doctors");
        }

        try
        {
            var result = await _adminApiService.GetDoctorScheduleAsync<Result<DoctorSchedule_DTO>>(Id);
            if (result?.IsSuccess == true && result.Data != null)
            {
                Schedule = result.Data;
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to load doctor schedule.";
                return RedirectToPage("/Admin/Doctors");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor schedule for ID: {Id}", Id);
            TempData["ErrorMessage"] = "An error occurred while loading the doctor schedule.";
            return RedirectToPage("/Admin/Doctors");
        }

        return Page();
    }
}
