using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.DoctorsDTOs;

/// <summary>
/// DTO for doctor registration containing professional and scheduling information
/// </summary>
public class RegisterDoctor_DTO
{
    /// <summary>
    /// Reference to the associated user account
    /// </summary>
    public string? UserID { get; set; }

    /// <summary>
    /// Medical specialization (e.g., Cardiology, Pediatrics)
    /// </summary>
    [Required(ErrorMessage = "Specialization is required")]
    public required string Specialization { get; set; }

    /// <summary>
    /// Arabic translation of specialization
    /// </summary>
    public string? ArabicSpecialization { get; set; }

    /// <summary>
    /// Clinic or office address
    /// </summary>
    public string? ClinicAddress { get; set; }

    /// <summary>
    /// Arabic translation of clinic address
    /// </summary>
    public string? ArabicClinicAddress { get; set; }

    /// <summary>
    /// Years of medical practice experience
    /// </summary>
    [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50")]
    public int? ExperienceYears { get; set; }

    /// <summary>
    /// Medical license number
    /// </summary>
    [Required(ErrorMessage = "License number is required")]
    public required string LicenseNumber { get; set; }

    /// <summary>
    /// Summary of qualifications (e.g., MBBS, MD, MS)
    /// </summary>
    public string? QualificationSummary { get; set; }

    /// <summary>
    /// Hospital or institution affiliation
    /// </summary>
    public string? HospitalAffiliation { get; set; }

    /// <summary>
    /// Medical department
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Board certifications
    /// </summary>
    public string? Certifications { get; set; }

    /// <summary>
    /// Days available for appointments (comma-separated: "Monday, Wednesday, Friday")
    /// </summary>
    [Required(ErrorMessage = "Available days are required")]
    public required string AvailableDays { get; set; }

    /// <summary>
    /// Start time for daily availability (e.g., "09:00")
    /// </summary>
    [Required(ErrorMessage = "Start time is required")]
    public required string StartTime { get; set; }

    /// <summary>
    /// End time for daily availability (e.g., "17:00")
    /// </summary>
    [Required(ErrorMessage = "End time is required")]
    public required string EndTime { get; set; }

    /// <summary>
    /// Appointment duration in minutes
    /// </summary>
    public int AppointmentDuration { get; set; } = 30;

    /// <summary>
    /// Maximum patients per day
    /// </summary>
    public int MaxPatientsPerDay { get; set; } = 20;

    /// <summary>
    /// List of detailed qualifications
    /// </summary>
    public List<Qualification_DTO>? Qualifications { get; set; }

    /// <summary>
    /// User ID of the person creating this record
    /// </summary>
    public required string CreatedBy { get; set; }
}
