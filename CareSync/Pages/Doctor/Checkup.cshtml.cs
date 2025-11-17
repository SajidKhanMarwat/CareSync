using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Doctor;

public class CheckupModel : BasePageModel
{
    private readonly ILogger<CheckupModel> _logger;

    public CheckupModel(ILogger<CheckupModel> logger)
    {
        _logger = logger;
    }

    // Patient Information
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactNumber { get; set; } = string.Empty;

    // Appointment Information
    public int AppointmentId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string AppointmentType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;

    // Current Vitals
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public int? PulseRate { get; set; }
    public string? BloodPressure { get; set; }
    public bool IsDiabetic { get; set; }
    public string? DiabeticReadings { get; set; }
    public bool HasHighBloodPressure { get; set; }
    public string? BloodPressureReadings { get; set; }

    // Chronic Diseases
    public List<CheckupChronicDiseaseDto> ChronicDiseases { get; set; } = new();

    // Medical History
    public List<PreviousPrescriptionDto> PreviousPrescriptions { get; set; } = new();
    public List<PreviousVitalDto> PreviousVitals { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int appointmentId)
    {
        // Check if user is authenticated and has Doctor role
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            await LoadCheckupData(appointmentId);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading checkup data for appointment {AppointmentId}", appointmentId);
            TempData["Error"] = "Unable to load checkup data. Please try again.";
            return RedirectToPage("/Doctor/Dashboard");
        }
    }

    private async Task LoadCheckupData(int appointmentId)
    {
        // Load appointment and patient data
        // This would typically come from database
        AppointmentId = appointmentId;
        
        // Mock patient data
        PatientId = 101;
        PatientName = "Willian Mathews";
        PatientAge = 21;
        Gender = "Male";
        BloodGroup = "O+";
        MaritalStatus = "Single";
        EmergencyContactName = "Sarah Mathews";
        EmergencyContactNumber = "+1234567890";

        // Mock appointment data
        AppointmentDate = DateTime.Now;
        AppointmentType = "General Consultation";
        Reason = "Heart Attack symptoms";

        // Mock current vitals
        Height = 175.5m;
        Weight = 75.0m;
        PulseRate = 72;
        BloodPressure = "120/80";
        IsDiabetic = false;
        DiabeticReadings = "";
        HasHighBloodPressure = false;
        BloodPressureReadings = "Previous: 118/78 (2 weeks ago)";

        // Mock chronic diseases
        ChronicDiseases = new List<CheckupChronicDiseaseDto>
        {
            new CheckupChronicDiseaseDto
            {
                DiseaseName = "Asthma",
                DiagnosedDate = DateTime.Now.AddYears(-2),
                CurrentStatus = "Controlled"
            }
        };

        // Mock previous prescriptions
        PreviousPrescriptions = new List<PreviousPrescriptionDto>
        {
            new PreviousPrescriptionDto
            {
                PrescriptionID = 1,
                DoctorName = "Dr. Jane Smith",
                CreatedOn = DateTime.Now.AddDays(-30),
                Notes = "For seasonal allergies",
                Medications = "Cetirizine 10mg, Once daily for 10 days"
            },
            new PreviousPrescriptionDto
            {
                PrescriptionID = 2,
                DoctorName = "Dr. John Doe",
                CreatedOn = DateTime.Now.AddDays(-60),
                Notes = "Common cold treatment",
                Medications = "Paracetamol 500mg, Three times daily for 5 days"
            }
        };

        // Mock previous vitals
        PreviousVitals = new List<PreviousVitalDto>
        {
            new PreviousVitalDto
            {
                RecordedDate = DateTime.Now.AddDays(-14),
                Height = 175.5m,
                Weight = 74.5m,
                BloodPressure = "118/78",
                PulseRate = 70,
                IsDiabetic = false
            },
            new PreviousVitalDto
            {
                RecordedDate = DateTime.Now.AddDays(-30),
                Height = 175.5m,
                Weight = 74.0m,
                BloodPressure = "120/80",
                PulseRate = 68,
                IsDiabetic = false
            }
        };
    }

    public async Task<IActionResult> OnPostUpdateVitalsAsync(
        int appointmentId, 
        int patientId,
        decimal? height,
        decimal? weight,
        int? pulseRate,
        string? bloodPressure,
        bool isDiabetic,
        string? diabeticReadings,
        bool hasHighBloodPressure,
        string? bloodPressureReadings,
        List<CheckupChronicDiseaseDto>? chronicDiseases)
    {
        try
        {
            // Here you would save the vitals to the database
            // For now, we'll just show a success message
            
            _logger.LogInformation("Updating vitals for patient {PatientId} in appointment {AppointmentId}", patientId, appointmentId);
            
            // TODO: Save to T_PatientVitals table
            // TODO: Update T_ChronicDiseases table
            
            TempData["Success"] = "Patient vitals and health information updated successfully!";
            return RedirectToPage("/Doctor/Checkup", new { appointmentId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vitals for patient {PatientId}", patientId);
            TempData["Error"] = "Failed to update vitals. Please try again.";
            return RedirectToPage("/Doctor/Checkup", new { appointmentId });
        }
    }

    public async Task<IActionResult> OnPostCreatePrescriptionAsync(
        int appointmentId,
        int patientId,
        List<MedicationDto> medications,
        string? prescriptionNotes)
    {
        try
        {
            _logger.LogInformation("Creating prescription for patient {PatientId} in appointment {AppointmentId}", patientId, appointmentId);
            
            // TODO: Save to T_Prescriptions table
            // TODO: Save medications to T_PrescriptionItems table
            
            TempData["Success"] = "Prescription created successfully!";
            return RedirectToPage("/Doctor/Checkup", new { appointmentId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating prescription for patient {PatientId}", patientId);
            TempData["Error"] = "Failed to create prescription. Please try again.";
            return RedirectToPage("/Doctor/Checkup", new { appointmentId });
        }
    }
}

// DTOs for form binding
public class CheckupChronicDiseaseDto
{
    public string? DiseaseName { get; set; }
    public DateTime? DiagnosedDate { get; set; }
    public string? CurrentStatus { get; set; }
}

public class MedicationDto
{
    public string MedicineName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string? Instructions { get; set; }
}

public class PreviousPrescriptionDto
{
    public int PrescriptionID { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string? Notes { get; set; }
    public string Medications { get; set; } = string.Empty;
}

public class PreviousVitalDto
{
    public DateTime RecordedDate { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public string? BloodPressure { get; set; }
    public int? PulseRate { get; set; }
    public bool IsDiabetic { get; set; }
}
