using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using System.Text.Json;

namespace CareSync.Pages.Admin;

public class PatientsModel : BasePageModel
{
    private readonly ILogger<PatientsModel> _logger;
    private readonly AdminApiService _adminApiService;

    public PatientsModel(ILogger<PatientsModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public List<PatientList_DTO> Patients { get; set; } = new();
    public PatientStats_DTO? PatientStats { get; set; }
    public List<DoctorList_DTO> Doctors { get; set; } = new();
    public PatientRegistrationTrends_DTO? RegistrationTrends { get; set; }
    public PatientAgeDistribution_DTO? AgeDistribution { get; set; }
    public PatientDemographics_DTO? Demographics { get; set; }
    public string? ErrorMessage { get; set; }
    public bool HasError { get; set; }
    
    // Search and filter properties
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? BloodGroup { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }
    
    // Pagination properties
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; } = 1;

    public async Task<IActionResult> OnGetAsync(int? page)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            CurrentPage = page ?? 1;
            _logger.LogInformation("Loading patients list - Page: {Page}, BloodGroup: {BloodGroup}, IsActive: {IsActive}, Search: {Search}", 
                CurrentPage, BloodGroup, IsActive, SearchTerm);

            // Load patients and stats in parallel
            var patientsTask = _adminApiService.GetAllPatientsAsync<Result<List<PatientList_DTO>>>(BloodGroup, IsActive);
            var statsTask = _adminApiService.GetPatientStatsAsync<Result<PatientStats_DTO>>();
            var doctorsTask = _adminApiService.GetAllDoctorsAsync<Result<List<DoctorList_DTO>>>(null, true);
            var trendsTask = _adminApiService.GetPatientRegistrationTrendsAsync<Result<PatientRegistrationTrends_DTO>>();
            var ageTask = _adminApiService.GetPatientAgeDistributionAsync<Result<PatientAgeDistribution_DTO>>();
            var demographicsTask = _adminApiService.GetPatientDemographicsAsync<Result<PatientDemographics_DTO>>();

            await Task.WhenAll(patientsTask, statsTask, doctorsTask, trendsTask, ageTask, demographicsTask);

            var patientsResult = await patientsTask;
            if (patientsResult?.IsSuccess == true && patientsResult.Data != null)
            {
                Patients = patientsResult.Data;
                
                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    var searchLower = SearchTerm.ToLower();
                    Patients = Patients.Where(p => 
                        p.FirstName.ToLower().Contains(searchLower) ||
                        p.LastName.ToLower().Contains(searchLower) ||
                        p.Email.ToLower().Contains(searchLower) ||
                        p.PhoneNumber.Contains(searchLower)
                    ).ToList();
                }
                
                // Calculate pagination
                TotalPages = (int)Math.Ceiling(Patients.Count / (double)PageSize);
                Patients = Patients
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }

            var statsResult = await statsTask;
            if (statsResult?.IsSuccess == true && statsResult.Data != null)
                PatientStats = statsResult.Data;

            var doctorsResult = await doctorsTask;
            if (doctorsResult?.IsSuccess == true && doctorsResult.Data != null)
                Doctors = doctorsResult.Data;

            var trendsResult = await trendsTask;
            if (trendsResult?.IsSuccess == true && trendsResult.Data != null)
                RegistrationTrends = trendsResult.Data;

            var ageResult = await ageTask;
            if (ageResult?.IsSuccess == true && ageResult.Data != null)
                AgeDistribution = ageResult.Data;

            var demographicsResult = await demographicsTask;
            if (demographicsResult?.IsSuccess == true && demographicsResult.Data != null)
                Demographics = demographicsResult.Data;

            _logger.LogInformation("Loaded {Count} patients", Patients.Count);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patients");
            ErrorMessage = "Failed to load patients list. Please try again.";
            HasError = true;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeletePatientAsync(string userId, int patientId)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Deleting patient: UserId={UserId}, PatientId={PatientId}", userId, patientId);

            // Call the delete API endpoint
            var result = await _adminApiService.DeletePatientAsync<Result<GeneralResponse>>(userId, patientId);
            
            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Patient deleted successfully.";
                _logger.LogInformation("Successfully deleted patient: {PatientId}", patientId);
            }
            else
            {
                TempData["ErrorMessage"] = result?.Data?.Message ?? "Failed to delete patient.";
                _logger.LogError("Failed to delete patient: {PatientId}", patientId);
            }

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting patient");
            TempData["ErrorMessage"] = "An error occurred while deleting the patient.";
            return RedirectToPage();
        }
    }
    
    public async Task<IActionResult> OnGetSearchAsync(string term)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(term))
                return new JsonResult(new List<PatientSearch_DTO>());

            var result = await _adminApiService.SearchPatientsAsync<Result<List<PatientSearch_DTO>>>(term);
            
            if (result?.IsSuccess == true && result.Data != null)
                return new JsonResult(result.Data);
            
            return new JsonResult(new List<PatientSearch_DTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients");
            return new JsonResult(new List<PatientSearch_DTO>());
        }
    }

    // Helper method to get assigned doctor name
    public string GetAssignedDoctorName(int patientId)
    {
        // This would ideally come from the patient's appointment data
        // For now, return a placeholder or the first available doctor
        if (Doctors.Any())
            return Doctors.First().FullName;
        return "Not Assigned";
    }
}
