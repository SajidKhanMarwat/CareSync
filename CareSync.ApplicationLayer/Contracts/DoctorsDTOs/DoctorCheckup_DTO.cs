using CareSync.Shared.Enums;
using CareSync.Shared.Enums.Appointment;

namespace CareSync.ApplicationLayer.Contracts.DoctorsDTOs;

public class DoctorCheckup_DTO
{
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
    public List<CheckupChronicDisease_DTO> ChronicDiseases { get; set; } = new();

    // Medical History
    public List<PreviousPrescription_DTO> PreviousPrescriptions { get; set; } = new();
    public List<PreviousVital_DTO> PreviousVitals { get; set; } = new();
}

public class CheckupChronicDisease_DTO
{
    public string? DiseaseName { get; set; }
    public DateTime? DiagnosedDate { get; set; }
    public string? CurrentStatus { get; set; }
}

public class Medication_DTO
{
    public string MedicineName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string? Instructions { get; set; }
}

public class PreviousPrescription_DTO
{
    public int PrescriptionID { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string? Notes { get; set; }
    public string Medications { get; set; } = string.Empty;
}

public class PreviousVital_DTO
{
    public DateTime RecordedDate { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public string? BloodPressure { get; set; }
    public int? PulseRate { get; set; }
    public bool IsDiabetic { get; set; }
}

public class DoctorUpdateVitals_DTO
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public int? PulseRate { get; set; }
    public string? BloodPressure { get; set; }
    public bool IsDiabetic { get; set; }
    public string? DiabeticReadings { get; set; }
    public bool HasHighBloodPressure { get; set; }
    public string? BloodPressureReadings { get; set; }
    public List<CheckupChronicDisease_DTO>? ChronicDiseases { get; set; }
}

public class DoctorCreatePrescription_DTO
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public List<Medication_DTO> Medications { get; set; } = new();
    public string? PrescriptionNotes { get; set; }
}
