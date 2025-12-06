using System;
using System.Collections.Generic;

namespace CareSync.ApplicationLayer.Contracts.DoctorsDTOs;

public class DoctorPatientMedicalHistory_DTO
{
    // Patient header
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Statistics
    public int TotalVisits { get; set; }
    public int TotalPrescriptions { get; set; }
    public int TotalLabTests { get; set; }
    public int TotalReports { get; set; }

    // Current vitals (latest)
    public DoctorMedicalHistoryVitalsDto? CurrentVitals { get; set; }

    // Timelines
    public List<DoctorMedicalHistoryVisitDto> VisitHistory { get; set; } = new();
    public List<DoctorMedicalHistoryPrescriptionDto> Prescriptions { get; set; } = new();
    public List<DoctorMedicalHistoryLabTestDto> LabTests { get; set; } = new();

    // Chronic conditions & allergies (optional for now)
    public List<DoctorMedicalHistoryChronicDiseaseDto> ChronicDiseases { get; set; } = new();
    public List<DoctorMedicalHistoryAllergyDto> Allergies { get; set; } = new();

    public string FamilyHistory { get; set; } = string.Empty;
    public string DoctorNotes { get; set; } = string.Empty;
}

public class DoctorMedicalHistoryVitalsDto
{
    public string BloodPressure { get; set; } = string.Empty;
    public int? HeartRate { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? BMI { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class DoctorMedicalHistoryVisitDto
{
    public int VisitID { get; set; }
    public DateTime VisitDate { get; set; }
    public string VisitType { get; set; } = string.Empty;
    public string ChiefComplaint { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class DoctorMedicalHistoryPrescriptionDto
{
    public int PrescriptionID { get; set; }
    public DateTime PrescriptionDate { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public List<string> Medications { get; set; } = new();
    public bool IsActive { get; set; }
}

public class DoctorMedicalHistoryLabTestDto
{
    public int TestID { get; set; }
    public DateTime TestDate { get; set; }
    public string TestType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsAbnormal { get; set; }
}

public class DoctorMedicalHistoryChronicDiseaseDto
{
    public int DiseaseID { get; set; }
    public string DiseaseName { get; set; } = string.Empty;
    public DateTime? DiagnosedDate { get; set; }
}

public class DoctorMedicalHistoryAllergyDto
{
    public int AllergyID { get; set; }
    public string AllergyName { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}
