using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using Microsoft.Extensions.Logging;

namespace CareSync.Pages.Admin.Patients;

public class PatientAppointmentsModel : BasePageModel
{
    private readonly ILogger<PatientAppointmentsModel> _logger;
    private readonly AdminApiService _adminApiService;

    public PatientAppointmentsModel(ILogger<PatientAppointmentsModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public PatientList_DTO? Patient { get; set; }
    public List<PatientAppointmentDto> Appointments { get; set; } = new();
    
    // Statistics
    public int TotalAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public int UpcomingAppointments { get; set; }
    public int CancelledAppointments { get; set; }
    
    // Pagination
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; } = 1;

    public async Task<IActionResult> OnGetAsync(int? patientId, int? page)
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

            CurrentPage = page ?? 1;
            _logger.LogInformation("Loading appointments for patient ID: {PatientId}, Page: {Page}", patientId.Value, CurrentPage);

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
                
                // TODO: Load actual appointments when API endpoint is available
                // For now, create sample data for demonstration
                LoadSampleAppointments();
                
                // Calculate statistics
                TotalAppointments = Appointments.Count;
                CompletedAppointments = Appointments.Count(a => a.Status == "Completed");
                UpcomingAppointments = Appointments.Count(a => a.Status == "Scheduled" || a.Status == "Confirmed");
                CancelledAppointments = Appointments.Count(a => a.Status == "Cancelled");
                
                // Apply pagination
                TotalPages = (int)Math.Ceiling(TotalAppointments / (double)PageSize);
                Appointments = Appointments
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
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
            _logger.LogError(ex, "Error loading patient appointments");
            TempData["ErrorMessage"] = "An error occurred while loading appointments";
            return RedirectToPage("/Admin/Patients/PatientsList");
        }
    }

    private void LoadSampleAppointments()
    {
        // Sample data for demonstration - will be replaced with actual API calls
        var random = new Random();
        var statuses = new[] { "Completed", "Scheduled", "Confirmed", "Cancelled", "Pending" };
        var types = new[] { "Consultation", "Follow-up", "Check-up", "Emergency" };
        var doctors = new[] 
        { 
            ("Dr. Smith", "Cardiology"),
            ("Dr. Johnson", "General Medicine"),
            ("Dr. Williams", "Pediatrics"),
            ("Dr. Brown", "Orthopedics")
        };
        
        Appointments = new List<PatientAppointmentDto>();
        
        for (int i = 1; i <= 25; i++)
        {
            var doctor = doctors[random.Next(doctors.Length)];
            var appointment = new PatientAppointmentDto
            {
                AppointmentID = i,
                AppointmentDate = DateTime.Now.AddDays(random.Next(-30, 30)),
                DoctorName = doctor.Item1,
                DoctorSpecialization = doctor.Item2,
                AppointmentType = types[random.Next(types.Length)],
                Reason = $"Regular {types[random.Next(types.Length)].ToLower()} appointment",
                Status = statuses[random.Next(statuses.Length)]
            };
            
            Appointments.Add(appointment);
        }
        
        // Sort by date descending
        Appointments = Appointments.OrderByDescending(a => a.AppointmentDate).ToList();
    }
}

// Temporary DTO - Should be moved to ApplicationLayer when implementing the actual API
public class PatientAppointmentDto
{
    public int AppointmentID { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialization { get; set; } = string.Empty;
    public string AppointmentType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
