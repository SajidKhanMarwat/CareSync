using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using Microsoft.Extensions.Logging;

namespace CareSync.Pages.Admin.Patients;

public class PatientMedicalRecordsModel : BasePageModel
{
    private readonly ILogger<PatientMedicalRecordsModel> _logger;
    private readonly AdminApiService _adminApiService;

    public PatientMedicalRecordsModel(ILogger<PatientMedicalRecordsModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public PatientList_DTO? Patient { get; set; }
    
    // Medical Records Data (These will be populated from API when endpoints are available)
    public List<VitalSignDto> Vitals { get; set; } = new();
    public List<MedicalHistoryDto> MedicalHistory { get; set; } = new();
    public List<PrescriptionDto> Prescriptions { get; set; } = new();
    public List<LabReportDto> LabReports { get; set; } = new();
    public List<ChronicDiseaseDto> ChronicDiseases { get; set; } = new();
    public List<AllergyDto> Allergies { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? patientId)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            if (!patientId.HasValue)
            {
                TempData["ErrorMessage"] = "Patient ID is required";
                return RedirectToPage("/Admin/Patients/PatientsList");
            }

            _logger.LogInformation("Loading medical records for patient ID: {PatientId}", patientId.Value);

            // Load patient information
            var patientsResult = await _adminApiService.GetAllPatientsAsync<Result<List<PatientList_DTO>>>(null, null);
            
            if (patientsResult?.IsSuccess == true && patientsResult.Data != null)
            {
                Patient = patientsResult.Data.FirstOrDefault(p => p.PatientID == patientId.Value);
                
                if (Patient == null)
                {
                    TempData["ErrorMessage"] = "Patient not found";
                    _logger.LogWarning("Patient with ID {PatientId} not found", patientId.Value);
                    return RedirectToPage("/Admin/Patients/PatientsList");
                }
                
                _logger.LogInformation("Successfully loaded patient: {PatientName}", Patient.FullName);
                
                // TODO: Load actual medical records when API endpoints are available
                // For now, create sample data for demonstration
                LoadSampleData();
            }
            else
            {
                TempData["ErrorMessage"] = patientsResult?.GetError() ?? "Failed to load patient data";
                _logger.LogError("Failed to load patient data");
                return RedirectToPage("/Admin/Patients/PatientsList");
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading medical records");
            TempData["ErrorMessage"] = "An error occurred while loading medical records";
            return RedirectToPage("/Admin/Patients/PatientsList");
        }
    }

    private void LoadSampleData()
    {
        // Sample data for demonstration - will be replaced with actual API calls
        Vitals = new List<VitalSignDto>
        {
            new VitalSignDto 
            { 
                Id = 1,
                RecordedDate = DateTime.Now.AddDays(-7),
                BloodPressure = "120/80",
                HeartRate = 72,
                Temperature = 98.6m,
                Weight = 70,
                Height = 175,
                BMI = 22.9m,
                RecordedBy = "Nurse Smith"
            }
        };

        MedicalHistory = new List<MedicalHistoryDto>
        {
            new MedicalHistoryDto
            {
                Id = 1,
                Date = DateTime.Now.AddMonths(-3),
                Condition = "Hypertension",
                Description = "Patient diagnosed with mild hypertension. Started on medication.",
                DoctorName = "Dr. Johnson"
            }
        };

        // Empty lists for other sections
        Prescriptions = new List<PrescriptionDto>();
        LabReports = new List<LabReportDto>();
        ChronicDiseases = new List<ChronicDiseaseDto>();
        Allergies = new List<AllergyDto>();
    }
}

// Temporary DTOs - These should be moved to the ApplicationLayer when implementing the actual API
public class VitalSignDto
{
    public int Id { get; set; }
    public DateTime RecordedDate { get; set; }
    public string BloodPressure { get; set; } = string.Empty;
    public int HeartRate { get; set; }
    public decimal Temperature { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public decimal BMI { get; set; }
    public string RecordedBy { get; set; } = string.Empty;
}

public class MedicalHistoryDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
}

public class PrescriptionDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public List<string> Medications { get; set; } = new();
    public string DoctorName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class LabReportDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string OrderedBy { get; set; } = string.Empty;
}

public class ChronicDiseaseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DiagnosedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class AllergyDto
{
    public int Id { get; set; }
    public string Allergen { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Reaction { get; set; } = string.Empty;
}
