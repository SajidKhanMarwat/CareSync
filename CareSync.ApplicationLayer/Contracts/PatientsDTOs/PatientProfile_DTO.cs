using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.Shared.Enums.Patient;

namespace CareSync.ApplicationLayer.Contracts.PatientsDTOs;

/// <summary>
/// Comprehensive DTO for patient profile with all details
/// </summary>
public class PatientProfile_DTO
{
    // Basic Information
    public int PatientID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string ProfileImage { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    
    // Address Information
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    
    // Medical Information
    public string? BloodGroup { get; set; }
    public MaritalStatusEnum MaritalStatus { get; set; }
    public string? Occupation { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public string? PreferredLanguage { get; set; }
    
    // Emergency Contact
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactNumber { get; set; }
    public string? RelationshipToEmergency { get; set; }
    public string? EmergencyContactAddress { get; set; }
    
    // Medical History Summary
    public List<string> Allergies { get; set; } = new();
    public List<string> ChronicDiseases { get; set; } = new();
    public List<string> CurrentMedications { get; set; } = new();
    public string? MedicalNotes { get; set; }
    
    // Statistics
    public int TotalAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public int UpcomingAppointments { get; set; }
    public int MissedAppointments { get; set; }
    public DateTime? LastVisit { get; set; }
    public DateTime? NextAppointment { get; set; }
    public string? PreferredDoctor { get; set; }
    
    // Recent Activity
    public List<Appointment_DTO> RecentAppointments { get; set; } = new();
    public List<PatientVital_DTO> RecentVitals { get; set; } = new();
    public List<Prescription_DTO> ActivePrescriptions { get; set; } = new();
    public List<LabReport_DTO> RecentLabReports { get; set; } = new();
}

/// <summary>
/// DTO for patient vital signs
/// </summary>
public class PatientVital_DTO
{
    public int VitalID { get; set; }
    public DateTime RecordedDate { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public string? BloodPressure { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Temperature { get; set; }
    public int? RespiratoryRate { get; set; }
    public decimal? BMI { get; set; }
    public string? RecordedBy { get; set; }
}

/// <summary>
/// DTO for prescription information
/// </summary>
public class Prescription_DTO
{
    public int PrescriptionID { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string PrescribedBy { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for lab report information
/// </summary>
public class LabReport_DTO
{
    public int ReportID { get; set; }
    public string TestName { get; set; } = string.Empty;
    public DateTime TestDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Result { get; set; }
    public string? ReferenceRange { get; set; }
    public string? OrderedBy { get; set; }
    public string? LabName { get; set; }
}
