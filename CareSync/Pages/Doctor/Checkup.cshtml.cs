using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.Shared.Enums;
using CareSync.Shared.Enums.Appointment;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareSync.Pages.Doctor;

public class CheckupModel : BasePageModel
{
    private readonly ILogger<CheckupModel> _logger;
    private readonly DoctorApiService _doctorApiService;

    public CheckupModel(ILogger<CheckupModel> logger, DoctorApiService doctorApiService)
    {
        _logger = logger;
        _doctorApiService = doctorApiService;
    }

    // Patient Information
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public Gender_Enum Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string MaritalStatus { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactNumber { get; set; } = string.Empty;

    // Appointment Information
    public int AppointmentId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentType_Enum AppointmentType { get; set; }
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
            // Always keep track of the requested appointment so the page can bind even on partial failure
            AppointmentId = appointmentId;
            var result = await _doctorApiService.GetCheckupAsync(appointmentId);
            if (result != null && result.IsSuccess && result.Data is not null)
            {
                BindFromDto(result.Data);
                return Page();
            }

            // Fallback: try to at least load basic appointment details so doctor can still write a prescription
            var basic = await _doctorApiService.GetAppointmentByIdAsync(appointmentId);
            if (basic != null && basic.IsSuccess && basic.Data is not null)
            {
                BindBasicFromAppointment(basic.Data);
                TempData["Warning"] = result?.GetError() ?? basic.GetError() ?? "Loaded limited checkup data for this appointment.";
                return Page();
            }

            TempData["Error"] = result?.GetError() ?? basic?.GetError() ?? "Unable to load checkup data.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading checkup data for appointment {AppointmentId}", appointmentId);
            TempData["Error"] = "Unable to load checkup data. Please try again.";
            return Page();
        }
    }

    private void BindFromDto(DoctorCheckup_DTO dto)
    {
        AppointmentId = dto.AppointmentId;
        AppointmentDate = dto.AppointmentDate;
        AppointmentType = dto.AppointmentType;
        Reason = dto.Reason;

        PatientId = dto.PatientId;
        PatientName = dto.PatientName;
        PatientAge = dto.PatientAge;
        Gender = dto.Gender;
        BloodGroup = dto.BloodGroup;
        MaritalStatus = dto.MaritalStatus;
        EmergencyContactName = dto.EmergencyContactName;
        EmergencyContactNumber = dto.EmergencyContactNumber;

        Height = dto.Height;
        Weight = dto.Weight;
        PulseRate = dto.PulseRate;
        BloodPressure = dto.BloodPressure;
        IsDiabetic = dto.IsDiabetic;
        DiabeticReadings = dto.DiabeticReadings;
        HasHighBloodPressure = dto.HasHighBloodPressure;
        BloodPressureReadings = dto.BloodPressureReadings;

        ChronicDiseases = dto.ChronicDiseases
            .Select(cd => new CheckupChronicDiseaseDto
            {
                DiseaseName = cd.DiseaseName,
                DiagnosedDate = cd.DiagnosedDate,
                CurrentStatus = cd.CurrentStatus
            })
            .ToList();

        PreviousPrescriptions = dto.PreviousPrescriptions
            .Select(p => new PreviousPrescriptionDto
            {
                PrescriptionID = p.PrescriptionID,
                DoctorName = p.DoctorName,
                CreatedOn = p.CreatedOn,
                Notes = p.Notes,
                Medications = p.Medications
            })
            .ToList();

        PreviousVitals = dto.PreviousVitals
            .Select(v => new PreviousVitalDto
            {
                RecordedDate = v.RecordedDate,
                Height = v.Height,
                Weight = v.Weight,
                BloodPressure = v.BloodPressure,
                PulseRate = v.PulseRate,
                IsDiabetic = v.IsDiabetic
            })
            .ToList();
    }

    private void BindBasicFromAppointment(AppointmentDetails_DTO dto)
    {
        AppointmentId = dto.AppointmentID;
        AppointmentDate = dto.AppointmentDate;
        AppointmentType = dto.AppointmentType;
        Reason = dto.Reason ?? string.Empty;

        PatientId = dto.PatientID;
        PatientName = dto.PatientName;
        PatientAge = dto.PatientAge;
        Gender = dto.Gender;
        BloodGroup = dto.BloodGroup;
        MaritalStatus = dto.MaritalStatus;
        EmergencyContactName = dto.PatientContact ?? string.Empty;
        EmergencyContactNumber = dto.PatientContact ?? string.Empty;
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
            _logger.LogInformation("Updating vitals for patient {PatientId} in appointment {AppointmentId}", patientId, appointmentId);

            var input = new DoctorUpdateVitals_DTO
            {
                AppointmentId = appointmentId,
                PatientId = patientId,
                Height = height,
                Weight = weight,
                PulseRate = pulseRate,
                BloodPressure = bloodPressure,
                IsDiabetic = isDiabetic,
                DiabeticReadings = diabeticReadings,
                HasHighBloodPressure = hasHighBloodPressure,
                BloodPressureReadings = bloodPressureReadings,
                ChronicDiseases = chronicDiseases?.Select(cd => new CheckupChronicDisease_DTO
                {
                    DiseaseName = cd.DiseaseName,
                    DiagnosedDate = cd.DiagnosedDate,
                    CurrentStatus = cd.CurrentStatus
                }).ToList()
            };

            var result = await _doctorApiService.UpdateVitalsAsync(input);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.GetError() ?? "Failed to update vitals. Please try again.";
            }
            else if (result.Data is not null)
            {
                TempData["Success"] = result.Data.Message;
            }

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

            if (appointmentId <= 0 || patientId <= 0)
            {
                TempData["Error"] = "Invalid appointment or patient information.";
                return RedirectToPage("/Doctor/Checkup", new { appointmentId });
            }

            if (medications == null || medications.Count == 0)
            {
                TempData["Error"] = "Please add at least one medication to create a prescription.";
                return RedirectToPage("/Doctor/Checkup", new { appointmentId });
            }

            var input = new DoctorCreatePrescription_DTO
            {
                AppointmentId = appointmentId,
                PatientId = patientId,
                Medications = medications.Select(m => new Medication_DTO
                {
                    MedicineName = m.MedicineName,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    DurationDays = m.DurationDays,
                    Instructions = m.Instructions
                }).ToList(),
                PrescriptionNotes = prescriptionNotes
            };

            var result = await _doctorApiService.CreatePrescriptionAsync(input);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.GetError() ?? "Failed to create prescription. Please try again.";
                return RedirectToPage("/Doctor/Checkup", new { appointmentId });
            }

            if (result.Data is not null)
            {
                TempData["Success"] = result.Data.Message;
            }
            return RedirectToPage("/Doctor/AppointmentDetails", new { id = appointmentId });
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
