using CareSync.Shared.Enums;
using CareSync.Shared.Enums.Appointment;
using CareSync.Shared.Enums.Patient;

namespace CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;

public class AppointmentDetails_DTO
{
    public int AppointmentID { get; set; }
    //public string AppointmentNumber { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public AppointmentType_Enum AppointmentType { get; set; }
    public AppointmentStatus_Enum Status { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedOn { get; set; }

    // Doctor
    public int DoctorID { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialization { get; set; } = string.Empty;

    // Patient
    public int PatientID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public string? PatientContact { get; set; }
    public string? PatientEmail { get; set; }

    // Medical notes
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? DoctorNotes { get; set; }

    // Vitals (optional)
    public string? BloodPressure { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Temperature { get; set; }

    // Related lists
    public List<string> Prescriptions { get; set; } = new();
    public List<string> LabReports { get; set; } = new();
    public Gender_Enum Gender { get; set; }
    public string? BloodGroup { get; set; }
    public MaritalStatusEnum MaritalStatus { get; set; }
}